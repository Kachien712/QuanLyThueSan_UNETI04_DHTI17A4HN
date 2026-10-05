// Họ và tên: Đào Duy Khánh
// Mã sinh viên: 23103100220
// Nội dung thực hiện: Module 2 - Lớp phân trang tổng quát sử dụng LINQ Skip() và Take()

using Microsoft.EntityFrameworkCore;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Helpers
{
    // Dùng Generic <T> để sau này bất kỳ danh sách nào (Sân, Bảng giá, Khách hàng) đều tái sử dụng được
    public class PaginatedList<T> : List<T>
    {
        public int TrangHienTai { get; private set; }
        public int TongSoTrang { get; private set; }
        public int TongSoBanGhi { get; private set; }
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            TrangHienTai = pageIndex;
            TongSoBanGhi = count;
            // Tính tổng số trang (làm tròn lên)
            TongSoTrang = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }
        // Kiểm tra xem có trang trước / trang sau để bật/tắt nút trên giao diện
        public bool CoTrangTruoc => TrangHienTai > 1;
        public bool CoTrangSau => TrangHienTai < TongSoTrang;
        // Hàm xử lý phân trang bất đồng bộ bằng EF Core
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            // Lấy dữ liệu trang hiện tại bằng Skip và Take của LINQ
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
