-- Script bổ sung các cột còn thiếu cho bảng DonDatHang
USE [BSDB2]
GO

-- Kiểm tra và thêm cột ThanhToan nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[dbo].[DonDatHang]') 
               AND name = 'ThanhToan')
BEGIN
    ALTER TABLE [dbo].[DonDatHang]
    ADD [ThanhToan] [int] NULL DEFAULT(1)
    PRINT 'Đã thêm cột ThanhToan'
END
ELSE
    PRINT 'Cột ThanhToan đã tồn tại'
GO

-- Kiểm tra và thêm cột Tracking nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[dbo].[DonDatHang]') 
               AND name = 'Tracking')
BEGIN
    ALTER TABLE [dbo].[DonDatHang]
    ADD [Tracking] [int] NULL DEFAULT(0)
    PRINT 'Đã thêm cột Tracking'
END
ELSE
    PRINT 'Cột Tracking đã tồn tại'
GO

-- Kiểm tra và thêm cột MaVoucher nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[dbo].[DonDatHang]') 
               AND name = 'MaVoucher')
BEGIN
    ALTER TABLE [dbo].[DonDatHang]
    ADD [MaVoucher] [int] NULL
    PRINT 'Đã thêm cột MaVoucher'
END
ELSE
    PRINT 'Cột MaVoucher đã tồn tại'
GO

-- Kiểm tra và thêm cột GiamGia nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[dbo].[DonDatHang]') 
               AND name = 'GiamGia')
BEGIN
    ALTER TABLE [dbo].[DonDatHang]
    ADD [GiamGia] [money] NULL DEFAULT(0)
    PRINT 'Đã thêm cột GiamGia'
END
ELSE
    PRINT 'Cột GiamGia đã tồn tại'
GO

-- Cập nhật dữ liệu cũ
UPDATE [dbo].[DonDatHang]
SET ThanhToan = 1, Tracking = 0, GiamGia = 0
WHERE ThanhToan IS NULL OR Tracking IS NULL OR GiamGia IS NULL
GO

PRINT 'Hoàn tất cập nhật bảng DonDatHang!'
GO
