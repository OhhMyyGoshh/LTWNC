-- Script tạo bảng Voucher và dữ liệu mẫu
USE [BSDB2]
GO

-- Tạo bảng Voucher
CREATE TABLE [dbo].[Voucher](
	[MaVoucher] [int] IDENTITY(1,1) NOT NULL,
	[MaCode] [varchar](20) NOT NULL,
	[TenVoucher] [nvarchar](100) NULL,
	[LoaiGiamGia] [bit] NULL, -- 0: Giảm %, 1: Giảm tiền cố định
	[GiaTriGiam] [decimal](18, 2) NULL,
	[GiaTriDonHangToiThieu] [money] NULL,
	[SoLuong] [int] NULL,
	[NgayBatDau] [datetime] NULL,
	[NgayKetThuc] [datetime] NULL,
	[TrangThai] [bit] NULL DEFAULT(1), -- 1: Hoạt động, 0: Ngưng
	PRIMARY KEY CLUSTERED ([MaVoucher] ASC)
)
GO

-- Thêm dữ liệu mẫu
INSERT INTO [dbo].[Voucher] 
([MaCode], [TenVoucher], [LoaiGiamGia], [GiaTriGiam], [GiaTriDonHangToiThieu], [SoLuong], [NgayBatDau], [NgayKetThuc], [TrangThai])
VALUES 
('GIAM10K', N'Giảm 10,000đ cho đơn từ 100k', 1, 10000, 100000, 100, GETDATE(), DATEADD(month, 3, GETDATE()), 1),
('GIAM15%', N'Giảm 15% cho đơn từ 200k', 0, 15, 200000, 50, GETDATE(), DATEADD(month, 2, GETDATE()), 1),
('FREESHIP', N'Giảm 30,000đ phí ship', 1, 30000, 0, 200, GETDATE(), DATEADD(month, 1, GETDATE()), 1),
('NEWUSER', N'Giảm 20% cho khách mới', 0, 20, 150000, 500, GETDATE(), DATEADD(year, 1, GETDATE()), 1)
GO

-- Tạo bảng lịch sử sử dụng voucher
CREATE TABLE [dbo].[VoucherSuDung](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[MaVoucher] [int] NOT NULL,
	[MaKH] [int] NOT NULL,
	[MaDDH] [int] NOT NULL,
	[NgaySuDung] [datetime] NULL DEFAULT(GETDATE()),
	PRIMARY KEY CLUSTERED ([ID] ASC),
	FOREIGN KEY([MaVoucher]) REFERENCES [dbo].[Voucher] ([MaVoucher]),
	FOREIGN KEY([MaKH]) REFERENCES [dbo].[KhachHang] ([MaKH]),
	FOREIGN KEY([MaDDH]) REFERENCES [dbo].[DonDatHang] ([MaDDH])
)
GO

-- Thêm cột để lưu mã voucher và giá trị giảm vào đơn hàng
ALTER TABLE [dbo].[DonDatHang]
ADD [MaVoucher] [int] NULL,
    [GiamGia] [money] NULL DEFAULT(0)
GO

ALTER TABLE [dbo].[DonDatHang]
ADD FOREIGN KEY([MaVoucher]) REFERENCES [dbo].[Voucher] ([MaVoucher])
GO

PRINT 'Đã tạo bảng Voucher và VoucherSuDung thành công!'
