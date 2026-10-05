namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Services;

public record OperationResult(bool ThanhCong, string ThongBao)
{
    public static OperationResult Success(string thongBao) => new(true, thongBao);
    public static OperationResult Failure(string thongBao) => new(false, thongBao);
}
