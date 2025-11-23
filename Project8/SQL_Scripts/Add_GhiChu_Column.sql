-- Thêm cột GhiChu vào bảng DonDatHang
USE [BSDB2]
GO

IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[dbo].[DonDatHang]') 
               AND name = 'GhiChu')
BEGIN
    ALTER TABLE [dbo].[DonDatHang]
    ADD [GhiChu] [NVARCHAR](500) NULL
    PRINT 'Đã thêm cột GhiChu vào bảng DonDatHang'
END
ELSE
BEGIN
    PRINT 'Cột GhiChu đã tồn tại'
END
GO
