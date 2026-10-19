// Họ và tên: Đào Đức Kiên
// Mã sinh viên: 23103100222
// Nội dung thực hiện: Module 3 - Controller Đặt sân & Tìm sân trống (Kiểm tra trùng lịch bằng LINQ, Validation thời gian và Action Create)

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Data;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Filters;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Helpers;
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

            // ===== COMMIT 4: VALIDATION NGHIỆP VỤ THỜI GIAN TRA CỨU SÂN =====
            bool coLoiValidation = false;

            // 1. Ngày sử dụng không được trong quá khứ
            if (model.NgaySuDung.Date < DateTime.Today)
            {
                ModelState.AddModelError(string.Empty, "Ngày sử dụng không được chọn trong quá khứ.");
                coLoiValidation = true;
            }

            // 2. Giờ kết thúc phải lớn hơn giờ bắt đầu
            if (model.GioKetThuc <= model.GioBatDau)
            {
                ModelState.AddModelError(string.Empty, "Giờ kết thúc phải lớn hơn giờ bắt đầu.");
                coLoiValidation = true;
            }
            else if ((model.GioKetThuc - model.GioBatDau).TotalMinutes < 30)
            {
                ModelState.AddModelError(string.Empty, "Thời lượng sử dụng sân tối thiểu là 30 phút.");
                coLoiValidation = true;
            }

            // 3. Khung giờ hoạt động cụm sân (05:00 - 23:00)
            if (model.GioBatDau < new TimeSpan(5, 0, 0) || model.GioKetThuc > new TimeSpan(23, 0, 0))
            {
                ModelState.AddModelError(string.Empty, "Thời gian đặt sân phải nằm trong khung giờ hoạt động của cụm sân (05:00 - 23:00).");
                coLoiValidation = true;
            }

            if (coLoiValidation)
            {
                model.DaTimKiem = false;
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

        // =========================================================================
        // 3. TẠO ĐƠN ĐẶT SÂN (GET - Nhận thông tin sân đã chọn từ TimSanTrong)
        // Yêu cầu đăng nhập thông qua [PhanQuyen] (Mục 5.3 & 17.1 file Word)
        // =========================================================================
        [HttpGet]
        [PhanQuyen]
        public async Task<IActionResult> Create(int? maSan, string? ngay, string? gioBatDau, string? gioKetThuc)
        {
            var viewModel = new DatSanCreateViewModel();

            // Tự động nhận diện tài khoản đang đăng nhập trong Session
            var maTaiKhoan = HttpContext.Session.GetInt32(SessionKeys.MaTaiKhoan);
            var vaiTro = HttpContext.Session.GetString(SessionKeys.VaiTro);

            if (maTaiKhoan.HasValue)
            {
                var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaTaiKhoan == maTaiKhoan.Value);
                if (khachHang != null)
                {
                    viewModel.MaKhachHang = khachHang.MaKhachHang;
                    ViewBag.TenKhachHangHienTai = khachHang.HoTen;
                    ViewBag.SoDienThoaiKhachHang = khachHang.SoDienThoai;
                }
            }

            ViewBag.LaKhachHang = (vaiTro == "KhachHang");

            // 1. Điền sẵn ngày sử dụng
            if (!string.IsNullOrWhiteSpace(ngay) && DateTime.TryParse(ngay, out DateTime parsedNgay))
            {
                viewModel.NgaySuDung = parsedNgay;
            }
            else
            {
                viewModel.NgaySuDung = DateTime.Today;
            }

            // 2. Điền sẵn giờ bắt đầu
            if (!string.IsNullOrWhiteSpace(gioBatDau) && TimeSpan.TryParse(gioBatDau, out TimeSpan parsedBatDau))
            {
                viewModel.GioBatDau = parsedBatDau;
            }
            else
            {
                viewModel.GioBatDau = new TimeSpan(8, 0, 0);
            }

            // 3. Điền sẵn giờ kết thúc
            if (!string.IsNullOrWhiteSpace(gioKetThuc) && TimeSpan.TryParse(gioKetThuc, out TimeSpan parsedKetThuc))
            {
                viewModel.GioKetThuc = parsedKetThuc;
            }
            else
            {
                viewModel.GioKetThuc = new TimeSpan(9, 30, 0);
            }

            // 4. Điền sẵn thông tin sân thể thao đã chọn
            if (maSan.HasValue)
            {
                var san = await _context.SanTheThaos
                    .Include(s => s.LoaiSan)
                    .Include(s => s.KhuVuc)
                    .FirstOrDefaultAsync(s => s.MaSan == maSan.Value && s.TinhTrang == "Đang hoạt động");

                if (san != null)
                {
                    viewModel.MaSan = san.MaSan;
                    viewModel.TenSanDuocChon = san.TenSan;
                    viewModel.TenLoaiSan = san.LoaiSan?.TenLoaiSan ?? "Tiêu chuẩn";
                    viewModel.TenKhuVuc = san.KhuVuc?.TenKhuVuc ?? "Chưa phân khu";
                    viewModel.SoNguoiToiDa = san.SucChua;
                    viewModel.SoNguoi = Math.Min(10, san.SucChua);
                }
            }

            // Nạp DropDown Khách hàng và Sân thể thao
            await NapDropdownDatSanAsync(viewModel);

            return View(viewModel);
        }

        // =========================================================================
        // HÀM HỖ TRỢ NẠP DROPDOWN
        // =========================================================================
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

        private async Task NapDropdownDatSanAsync(DatSanCreateViewModel viewModel)
        {
            var khachHangs = await _context.KhachHangs
                .Where(k => k.TrangThai)
                .OrderBy(k => k.HoTen)
                .ToListAsync();

            var sans = await _context.SanTheThaos
                .Where(s => s.TinhTrang == "Đang hoạt động")
                .OrderBy(s => s.TenSan)
                .ToListAsync();

            viewModel.DanhSachKhachHang = new SelectList(khachHangs, "MaKhachHang", "HoTen", viewModel.MaKhachHang);
            viewModel.DanhSachSan = new SelectList(sans, "MaSan", "TenSan", viewModel.MaSan);
        }
    }
}
