// Họ và tên: Đào Đức Kiên
// Mã sinh viên: 23103100222
// Nội dung thực hiện: Module 3 - Controller Đặt sân & Tìm sân trống (Kiểm tra trùng lịch bằng LINQ)

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Data;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;
using QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Controllers
{
    public class DatSanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DatSanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================================
        // 1. THUẬT TOÁN LINQ KIỂM TRA TRÙNG LỊCH (BẮT BUỘC - Mục 7.4 & 24.1 file Word)
        // Biểu thức giao nhau giữa 2 khoảng thời gian:
        // BatDauMoi < KetThucCu && KetThucMoi > BatDauCu
        // =========================================================================
        private async Task<bool> KiemTraTrungLichAsync(int maSan, DateTime ngaySuDung, TimeSpan gioBatDau, TimeSpan gioKetThuc, int? maDatSanHienTai = null)
        {
            var query = _context.DatSans
                .Where(d => d.MaSan == maSan
                         && d.NgaySuDung.Date == ngaySuDung.Date
                         && (d.TrangThai == "Chờ xác nhận" || d.TrangThai == "Đã xác nhận" || d.TrangThai == "Đang sử dụng"));

            // Nếu là cập nhật đơn thì loại trừ chính đơn đang xét
            if (maDatSanHienTai.HasValue)
            {
                query = query.Where(d => d.MaDatSan != maDatSanHienTai.Value);
            }

            // Hai khoảng thời gian giao nhau nếu: Bắt đầu mới < Kết thúc cũ VÀ Kết thúc mới > Bắt đầu cũ
            return await query.AnyAsync(d => gioBatDau < d.GioKetThuc && gioKetThuc > d.GioBatDau);
        }

        // =========================================================================
        // 2. CHỨC NĂNG TÌM SÂN TRỐNG (Mục 7.5 file Word - Xử lý Server-side bằng LINQ)
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> TimSanTrong(TimSanTrongViewModel model)
        {
            // Nạp danh sách Loại sân và Khu vực vào DropDown
            await NapDropdownLocSanAsync(model);

            // Mặc định ban đầu nếu chưa chọn ngày giờ thì gợi ý khung giờ chuẩn
            if (model.NgaySuDung == default)
            {
                model.NgaySuDung = DateTime.Today;
            }
            if (model.GioBatDau == default)
            {
                model.GioBatDau = new TimeSpan(8, 0, 0); // 08:00
            }
            if (model.GioKetThuc == default)
            {
                model.GioKetThuc = new TimeSpan(9, 30, 0); // 09:30
            }

            // Kiểm tra tính hợp lệ cơ bản của khung giờ
            if (model.GioKetThuc <= model.GioBatDau)
            {
                ModelState.AddModelError(string.Empty, "Giờ kết thúc phải lớn hơn giờ bắt đầu.");
                return View(model);
            }

            // Truy vấn lấy danh sách sân thể thao đang hoạt động
            var querySan = _context.SanTheThaos
                .Include(s => s.LoaiSan)
                .Include(s => s.KhuVuc)
                .Include(s => s.BangGias)
                .Where(s => s.TinhTrang == "Đang hoạt động")
                .AsQueryable();

            // Lọc theo Loại sân nếu người dùng chọn
            if (model.MaLoaiSan.HasValue)
            {
                querySan = querySan.Where(s => s.MaLoaiSan == model.MaLoaiSan.Value);
            }

            // Lọc theo Khu vực nếu người dùng chọn
            if (model.MaKhuVuc.HasValue)
            {
                querySan = querySan.Where(s => s.MaKhuVuc == model.MaKhuVuc.Value);
            }

            // Lọc theo Sức chứa nếu có yêu cầu số người
            if (model.SoNguoiYeuCau.HasValue && model.SoNguoiYeuCau.Value > 0)
            {
                querySan = querySan.Where(s => s.SucChua >= model.SoNguoiYeuCau.Value);
            }

            var danhSachSanHoatDong = await querySan.ToListAsync();
            var ketQuaSanTrong = new List<SanTrongItemViewModel>();

            // Duyệt từng sân và kiểm tra trùng lịch bằng LINQ
            foreach (var san in danhSachSanHoatDong)
            {
                bool biTrung = await KiemTraTrungLichAsync(san.MaSan, model.NgaySuDung, model.GioBatDau, model.GioKetThuc);
                if (!biTrung)
                {
                    // Tự động tìm bảng giá phù hợp đang có hiệu lực trong khung giờ này
                    var bangGia = san.BangGias
                        .Where(bg => bg.TrangThai
                                  && bg.NgayApDungTu.Date <= model.NgaySuDung.Date
                                  && bg.NgayApDungDen.Date >= model.NgaySuDung.Date
                                  && model.GioBatDau < bg.GioKetThuc && model.GioKetThuc > bg.GioBatDau)
                        .OrderByDescending(bg => bg.DonGiaMoiGio)
                        .FirstOrDefault();

                    ketQuaSanTrong.Add(new SanTrongItemViewModel
                    {
                        MaSan = san.MaSan,
                        TenSan = san.TenSan,
                        TenLoaiSan = san.LoaiSan?.TenLoaiSan ?? "Tiêu chuẩn",
                        TenKhuVuc = san.KhuVuc?.TenKhuVuc ?? "Chưa phân khu",
                        SoNguoiToiDa = san.SucChua,
                        GiaThamKhao = bangGia?.DonGiaMoiGio,
                        GhiChu = san.MoTa
                    });
                }
            }

            model.DanhSachSanTrong = ketQuaSanTrong;
            model.DaTimKiem = true;

            return View(model);
        }

        // Hàm nạp danh sách chọn vào dropdown
        private async Task NapDropdownLocSanAsync(TimSanTrongViewModel model)
        {
            var loaiSans = await _context.LoaiSans
                .Where(l => l.TrangThai)
                .OrderBy(l => l.TenLoaiSan)
                .ToListAsync();

            var khuVucs = await _context.KhuVucs
                .Where(k => k.TrangThai)
                .OrderBy(k => k.TenKhuVuc)
                .ToListAsync();

            model.DanhSachLoaiSan = new SelectList(loaiSans, "MaLoaiSan", "TenLoaiSan", model.MaLoaiSan);
            model.DanhSachKhuVuc = new SelectList(khuVucs, "MaKhuVuc", "TenKhuVuc", model.MaKhuVuc);
        }
    }
}
