-- Script thêm/cập nhật cột TrangThaiDonHang
USE [BSDB2]
GO

-- Xóa cột Tracking cũ (không dùng nữa)
IF EXISTS (SELECT * FROM sys.columns 
           WHERE object_id = OBJECT_ID(N'[dbo].[DonDatHang]') 
           AND name = 'Tracking')
BEGIN
    ALTER TABLE [dbo].[DonDatHang]
    DROP COLUMN [Tracking]
    PRINT 'Đã xóa cột Tracking cũ'
END
GO

-- Thêm cột TrangThaiDonHang mới
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[dbo].[DonDatHang]') 
               AND name = 'TrangThaiDonHang')
BEGIN
    ALTER TABLE [dbo].[DonDatHang]
    ADD [TrangThaiDonHang] [int] NULL DEFAULT(0)
    PRINT 'Đã thêm cột TrangThaiDonHang'
END
GO

-- Cập nhật dữ liệu cũ
-- 0: Chờ xác nhận
-- 1: Đã xác nhận
-- 2: Đang giao hàng
-- 3: Đã giao
-- 4: Đã hủy
UPDATE [dbo].[DonDatHang]
SET TrangThaiDonHang = 0
WHERE TrangThaiDonHang IS NULL
GO

PRINT 'Cấu trúc trạng thái đơn hàng:'
PRINT '0 = Chờ xác nhận'
PRINT '1 = Đã xác nhận'
PRINT '2 = Đang giao hàng'
PRINT '3 = Đã giao'
PRINT '4 = Đã hủy'
GO
