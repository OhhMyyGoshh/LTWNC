-- Kiểm tra và xóa đơn hàng test
USE [BSDB2]
GO

-- Xem tất cả đơn hàng
SELECT MaDDH, MaKH, NgayDat, TrangThaiDonHang, 
       (SELECT COUNT(*) FROM ChiTietDDH WHERE MaDDH = d.MaDDH) as SoSanPham
FROM DonDatHang d
ORDER BY MaDDH DESC
GO

-- Xóa đơn hàng cụ thể (thay số 1, 2 bằng MaDDH muốn xóa)
-- Uncomment 2 dòng dưới và thay số để xóa
-- DELETE FROM ChiTietDDH WHERE MaDDH IN (1, 2)
-- DELETE FROM DonDatHang WHERE MaDDH IN (1, 2)
GO
