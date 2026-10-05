# QuanLyThueSan_UNETI04_DHTI17A4HN

Ứng dụng ASP.NET Core 10 MVC quản lý sân thể thao và đặt lịch sử dụng sân.

## Module 4 - Lê Ngọc Hải Nam - 23103100219

- Xác nhận hoặc từ chối đơn chờ xác nhận; kiểm tra lại sân, khoảng giờ và lịch đã xác nhận trước khi chuyển trạng thái.
- Nhận sân chỉ khi đơn đã xác nhận; trả sân chỉ khi đơn đang sử dụng.
- Thêm dịch vụ chỉ khi đang sử dụng sân; hệ thống lấy đơn giá từ CSDL, chốt thành tiền và kiểm tra tồn kho nếu có.
- Quyền vận hành được kiểm tra trong Controller bằng Session `VaiTro` với các giá trị `Admin`, `NhanVien` hoặc `NhanVienQuanLy`.

## Khởi tạo cơ sở dữ liệu

1. Cập nhật chuỗi kết nối `QuanLySanConnection` trong `appsettings.json` cho SQL Server của nhóm.
2. Khôi phục công cụ và package: `dotnet tool restore` rồi `dotnet restore`.
3. Tạo database: `dotnet tool run dotnet-ef database update`.
4. Chạy ứng dụng: `dotnet run`.

Migration `KhoiTaoModule4` hiện tạo các bảng tối thiểu phục vụ Module 4. Khi tích hợp, thành viên Module 2 và 3 bổ sung thuộc tính vào Entity sân/đặt sân đã dùng chung thay vì tạo Entity trùng tên.
