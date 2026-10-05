/*
 * Họ và tên: Lê Ngọc Hải Nam
 * Mã sinh viên: 23103100219
 * Nội dung thực hiện: Hợp đồng xử lý xác nhận, nhận/trả sân và thêm dịch vụ sử dụng.
 */
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models.Enums;
using QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Services;

public interface IVanHanhSanService
{
    Task<IReadOnlyList<VanHanhDatSanViewModel>> GetDanhSachAsync(TrangThaiDatSan? trangThai, CancellationToken cancellationToken = default);
    Task<VanHanhDatSanViewModel?> GetChiTietAsync(int maDatSan, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SelectListItem>> GetDichVuKhaDungAsync(CancellationToken cancellationToken = default);
    Task<OperationResult> XacNhanAsync(int maDatSan, CancellationToken cancellationToken = default);
    Task<OperationResult> TuChoiAsync(int maDatSan, string lyDo, CancellationToken cancellationToken = default);
    Task<OperationResult> NhanSanAsync(int maDatSan, CancellationToken cancellationToken = default);
    Task<OperationResult> ThemDichVuAsync(int maDatSan, int maDichVu, int soLuong, CancellationToken cancellationToken = default);
    Task<OperationResult> TraSanAsync(int maDatSan, CancellationToken cancellationToken = default);
}
