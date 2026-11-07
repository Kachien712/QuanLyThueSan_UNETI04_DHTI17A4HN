// Họ và tên: Đào Đức Kiên
// Mã sinh viên: 23103100222
// Nội dung thực hiện: Module 3 - Controller Đặt sân & Tìm sân trống
// - Commit 1: Thuật toán kiểm tra trùng lịch LINQ
// - Commit 2, 3, 4: Tìm sân trống và validation thời gian
// - Commit 5, 6, 7, 8: Action Create GET/POST tạo đơn đặt sân
// - Commit 9, 10: Tra cứu lịch sử đặt sân của khách hàng (LichSuDatSan)
// - Commit 11, 12: Xem chi tiết đơn đặt sân (Details)
// - Commit 13: Hủy đặt sân (HuyDatSan)

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

            // Validation nghiệp vụ thời gian tra cứu sân
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

            if (model.MaLoaiSan.HasValue)
            {
                querySan = querySan.Where(s => s.MaLoaiSan == model.MaLoaiSan.Value);
            }

            if (model.MaKhuVuc.HasValue)
            {
                querySan = querySan.Where(s => s.MaKhuVuc == model.MaKhuVuc.Value);
            }

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
        // 4. XỬ LÝ LƯU ĐƠN ĐẶT SÂN (POST - Commit STT 7 & 8)
        // Kiểm tra 7 quy tắc nghiệp vụ EF Core & LINQ
        // =========================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PhanQuyen]
        public async Task<IActionResult> Create(DatSanCreateViewModel model)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32(SessionKeys.MaTaiKhoan);
            var vaiTro = HttpContext.Session.GetString(SessionKeys.VaiTro);
            ViewBag.LaKhachHang = (vaiTro == "KhachHang");

            // Nếu người dùng là Khách hàng, luôn cưỡng chế MaKhachHang theo Session tài khoản
            if (vaiTro == "KhachHang" && maTaiKhoan.HasValue)
            {
                var khachHangDangNhap = await _context.KhachHangs
                    .FirstOrDefaultAsync(k => k.MaTaiKhoan == maTaiKhoan.Value);

                if (khachHangDangNhap != null)
                {
                    model.MaKhachHang = khachHangDangNhap.MaKhachHang;
                    ViewBag.TenKhachHangHienTai = khachHangDangNhap.HoTen;
                    ViewBag.SoDienThoaiKhachHang = khachHangDangNhap.SoDienThoai;
                }
            }

            // 1. Kiểm tra khách hàng có hợp lệ và đang hoạt động không
            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaKhachHang == model.MaKhachHang);
            if (khachHang == null || !khachHang.TrangThai)
            {
                ModelState.AddModelError("MaKhachHang", "Khách hàng không tồn tại hoặc đã bị khóa tài khoản.");
            }

            // 2. Kiểm tra sân có hợp lệ và đang hoạt động không
            var san = await _context.SanTheThaos
                .Include(s => s.LoaiSan)
                .Include(s => s.KhuVuc)
                .FirstOrDefaultAsync(s => s.MaSan == model.MaSan);

            if (san == null || san.TinhTrang != "Đang hoạt động")
            {
                ModelState.AddModelError("MaSan", "Sân thể thao được chọn hiện không khả dụng hoặc đang bảo trì.");
            }
            else
            {
                model.TenSanDuocChon = san.TenSan;
                model.TenLoaiSan = san.LoaiSan?.TenLoaiSan ?? "Tiêu chuẩn";
                model.TenKhuVuc = san.KhuVuc?.TenKhuVuc ?? "Chưa phân khu";
                model.SoNguoiToiDa = san.SucChua;

                // 3. Kiểm tra số người không vượt quá sức chứa tối đa của sân
                if (model.SoNguoi > san.SucChua)
                {
                    ModelState.AddModelError("SoNguoi", $"Số người tham gia ({model.SoNguoi} người) vượt quá sức chứa tối đa của sân {san.TenSan} ({san.SucChua} người).");
                }
            }

            // 4. Kiểm tra ngày sử dụng không được trong quá khứ
            if (model.NgaySuDung.Date < DateTime.Today)
            {
                ModelState.AddModelError("NgaySuDung", "Ngày sử dụng không được ở trong quá khứ.");
            }

            // 5. Kiểm tra thời lượng và giờ mở cửa (05:00 - 23:00, tối thiểu 30 phút)
            var gioMoCua = new TimeSpan(5, 0, 0);
            var gioDongCua = new TimeSpan(23, 0, 0);

            if (model.GioBatDau < gioMoCua || model.GioKetThuc > gioDongCua)
            {
                ModelState.AddModelError(string.Empty, "Sân chỉ hoạt động trong khung giờ từ 05:00 đến 23:00 hàng ngày.");
            }

            if (model.GioKetThuc <= model.GioBatDau)
            {
                ModelState.AddModelError("GioKetThuc", "Giờ kết thúc phải lớn hơn giờ bắt đầu.");
            }
            else if ((model.GioKetThuc - model.GioBatDau).TotalMinutes < 30)
            {
                ModelState.AddModelError("GioKetThuc", "Thời lượng thuê sân tối thiểu phải từ 30 phút trở lên.");
            }

            if (model.NgaySuDung.Date == DateTime.Today && model.GioBatDau <= DateTime.Now.TimeOfDay)
            {
                ModelState.AddModelError("GioBatDau", "Giờ bắt đầu phải lớn hơn thời điểm hiện tại.");
            }

            // 6. Kiểm tra trùng lịch bằng LINQ qua hàm KiemTraTrungLichAsync
            if (ModelState.IsValid)
            {
                bool biTrungLich = await KiemTraTrungLichAsync(model.MaSan, model.NgaySuDung, model.GioBatDau, model.GioKetThuc);
                if (biTrungLich)
                {
                    ModelState.AddModelError(string.Empty, $"Khung giờ từ {model.GioBatDau:hh\\:mm} đến {model.GioKetThuc:hh\\:mm} ngày {model.NgaySuDung:dd/MM/yyyy} tại sân {san?.TenSan} đã có người đặt trước. Vui lòng chọn khung giờ hoặc sân khác.");
                }
            }

            if (!ModelState.IsValid)
            {
                await NapDropdownDatSanAsync(model);
                return View(model);
            }

            // 7. Tạo đối tượng DatSan và lưu vào cơ sở dữ liệu
            var donDatSan = new DatSan
            {
                MaKhachHang = model.MaKhachHang,
                MaSan = model.MaSan,
                NgaySuDung = model.NgaySuDung.Date,
                GioBatDau = model.GioBatDau,
                GioKetThuc = model.GioKetThuc,
                NgayDat = DateTime.Now,
                SoNguoi = model.SoNguoi,
                TrangThai = "Chờ xác nhận",
                GhiChu = model.GhiChu?.Trim()
            };

            _context.DatSans.Add(donDatSan);
            await _context.SaveChangesAsync();

            TempData["ThanhCong"] = $"Tạo đơn đặt sân thành công (Mã đơn: #{donDatSan.MaDatSan})! Trạng thái: Chờ xác nhận.";

            // Điều hướng sang trang Lịch sử đặt sân của tôi (Commit 9 & 10)
            return RedirectToAction(nameof(LichSuDatSan));
        }

        // =========================================================================
        // 5. COMMIT 9 & 10: TRA CỨU LỊCH SỬ ĐẶT SÂN CỦA KHÁCH HÀNG (LichSuDatSan)
        // Action GET lọc danh sách đặt sân theo MaKhachHang đang đăng nhập
        // =========================================================================
        [HttpGet]
        [PhanQuyen]
        public async Task<IActionResult> LichSuDatSan(string? trangThaiFilter)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32(SessionKeys.MaTaiKhoan);
            var vaiTro = HttpContext.Session.GetString(SessionKeys.VaiTro);

            var query = _context.DatSans
                .Include(d => d.KhachHang)
                .Include(d => d.SanTheThao).ThenInclude(s => s.LoaiSan)
                .Include(d => d.SanTheThao).ThenInclude(s => s.KhuVuc)
                .AsNoTracking()
                .AsQueryable();

            // Nếu người dùng là Khách hàng: Luôn lọc danh sách theo MaKhachHang của chính họ
            if (vaiTro == "KhachHang" && maTaiKhoan.HasValue)
            {
                var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaTaiKhoan == maTaiKhoan.Value);
                if (khachHang != null)
                {
                    query = query.Where(d => d.MaKhachHang == khachHang.MaKhachHang);
                    ViewBag.TenKhachHang = khachHang.HoTen;
                }
                else
                {
                    query = query.Where(d => false);
                }
            }

            ViewBag.VaiTro = vaiTro;
            ViewBag.LaKhachHang = (vaiTro == "KhachHang");

            // Lọc theo trạng thái nếu có
            if (!string.IsNullOrWhiteSpace(trangThaiFilter))
            {
                query = query.Where(d => d.TrangThai == trangThaiFilter);
            }

            var danhSach = await query
                .OrderByDescending(d => d.NgayDat)
                .ThenByDescending(d => d.NgaySuDung)
                .ToListAsync();

            ViewBag.TrangThaiFilter = trangThaiFilter;

            return View(danhSach);
        }

        // Action phụ trợ: Nếu người dùng truy cập /DatSan/Index thì tự động điều hướng sang LichSuDatSan
        [HttpGet]
        [PhanQuyen]
        public IActionResult Index() => RedirectToAction(nameof(LichSuDatSan));

        // =========================================================================
        // 6. COMMIT 11 & 12: XEM CHI TIẾT ĐƠN ĐẶT SÂN (Details)
        // Hiển thị đầy đủ thông tin: Sân, giờ thuê, khách hàng, thông tin dịch vụ kèm theo
        // =========================================================================
        [HttpGet]
        [PhanQuyen]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donDatSan = await _context.DatSans
                .Include(d => d.KhachHang)
                .Include(d => d.SanTheThao).ThenInclude(s => s.LoaiSan)
                .Include(d => d.SanTheThao).ThenInclude(s => s.KhuVuc)
                .Include(d => d.SuDungDichVus).ThenInclude(s => s.DichVu)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MaDatSan == id.Value);

            if (donDatSan == null)
            {
                return NotFound();
            }

            var maTaiKhoan = HttpContext.Session.GetInt32(SessionKeys.MaTaiKhoan);
            var vaiTro = HttpContext.Session.GetString(SessionKeys.VaiTro);

            // Bảo mật: Khách hàng chỉ được xem đơn của chính mình
            if (vaiTro == "KhachHang")
            {
                if (donDatSan.KhachHang == null || donDatSan.KhachHang.MaTaiKhoan != maTaiKhoan)
                {
                    TempData["Loi"] = "Bạn không có quyền xem thông tin đơn đặt sân của khách hàng khác.";
                    return RedirectToAction(nameof(LichSuDatSan));
                }
            }

            // Kiểm tra quy định hủy đơn (Commit 13):
            // Khách hàng chỉ được hủy khi đơn ở trạng thái "Chờ xác nhận" và trước giờ bắt đầu tối thiểu 2 tiếng
            var (choPhepKhachHuy, lyDoKhongChoHuy) = DatSanRuleHelper.KiemTraDieuKienHuy(
                donDatSan.TrangThai, donDatSan.NgaySuDung, donDatSan.GioBatDau, DateTime.Now);

            ViewBag.ChoPhepKhachHuy = choPhepKhachHuy;
            ViewBag.LyDoKhongChoHuy = lyDoKhongChoHuy;

            return View(donDatSan);
        }

        // =========================================================================
        // 7. COMMIT 13: HỦY ĐẶT SÂN (HuyDatSan)
        // Khách hàng chỉ được hủy khi đơn ở trạng thái "Chờ xác nhận" và trước giờ bắt đầu tối thiểu 2 tiếng
        // Cập nhật trạng thái thành "Đã hủy"
        // =========================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PhanQuyen]
        public async Task<IActionResult> HuyDatSan(int id, string? lyDoHuy)
        {
            var donDatSan = await _context.DatSans
                .Include(d => d.KhachHang)
                .FirstOrDefaultAsync(d => d.MaDatSan == id);

            if (donDatSan == null)
            {
                return NotFound();
            }

            var maTaiKhoan = HttpContext.Session.GetInt32(SessionKeys.MaTaiKhoan);
            var vaiTro = HttpContext.Session.GetString(SessionKeys.VaiTro);

            // Bảo mật: Khách hàng chỉ được hủy đơn của chính mình
            if (vaiTro == "KhachHang")
            {
                if (donDatSan.KhachHang == null || donDatSan.KhachHang.MaTaiKhoan != maTaiKhoan)
                {
                    TempData["Loi"] = "Bạn không có quyền hủy đơn đặt sân của khách hàng khác.";
                    return RedirectToAction(nameof(LichSuDatSan));
                }
            }

            // Kiểm tra điều kiện hủy theo quy định Commit 13
            var (choPhep, lyDo) = DatSanRuleHelper.KiemTraDieuKienHuy(
                donDatSan.TrangThai, donDatSan.NgaySuDung, donDatSan.GioBatDau, DateTime.Now);

            if (!choPhep)
            {
                TempData["Loi"] = lyDo;
                return RedirectToAction(nameof(Details), new { id });
            }

            // Cập nhật trạng thái thành "Đã hủy"
            donDatSan.TrangThai = "Đã hủy";
            string thongTinHuy = $"[Đã hủy lúc {DateTime.Now:HH:mm dd/MM/yyyy}]: {(string.IsNullOrWhiteSpace(lyDoHuy) ? "Khách hàng yêu cầu hủy đơn" : lyDoHuy.Trim())}";
            donDatSan.GhiChu = string.IsNullOrWhiteSpace(donDatSan.GhiChu)
                ? thongTinHuy
                : $"{donDatSan.GhiChu} | {thongTinHuy}";

            _context.Update(donDatSan);
            await _context.SaveChangesAsync();

            TempData["ThanhCong"] = $"Hủy đơn đặt sân #{donDatSan.MaDatSan} thành công!";
            return RedirectToAction(nameof(LichSuDatSan));
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
