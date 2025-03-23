--EcommerceDWA database



-- 1-to-N
CREATE TABLE [Category]
(
    ID_Category int identity primary key,
	[Name] nvarchar(150) NULL,
);

-- M-to-N
CREATE TABLE [Country]
(
    ID_Country int identity primary key,
	[Name] nvarchar(150) NULL,
);
go

-- Primary
CREATE TABLE [Product]
(
    ID_Product int identity primary key,
	[Name] nvarchar(150) NULL,
	[Description] nvarchar(150) NULL,
	[Category_ID] int null, --foreign x
    [Price] money NULL,
	[Img_URL] nvarchar(150) null,
	IsAvailable bit null default 1
);
go

-- Payment method (card, cash)
CREATE TABLE [PaymentMethod]
(
	ID_PaymentMethod int identity primary key,
	[Name] nvarchar(50),
);
go

-- Customer
CREATE TABLE [Customer]
(
    ID_Customer int identity primary key,
	[Name] nvarchar(50) NOT NULL,
	[Surname] nvarchar(50) not null,
	RegisteredAt datetime not null default GETUTCDATE()
);
go

-- User-M-to-N-bridge
CREATE TABLE [Order]
(
    ID_Order int identity primary key,
	[Customer_ID] int NOT NULL,
	PaymentMethod_ID int NOT NULL, --foreign x
	CreatedAt datetime not null default GETUTCDATE(),
	Total money null
);
go

-- M-N Customer
CREATE TABLE [Product-Order]
(
    ID_POrder int identity primary key,
	[Order_ID] int NOT NULL, --foreign x
	[Product_ID] int not null, --foreign x
	[Quantity] int not null
);
go

-- M-to-N-bridge
CREATE TABLE [Country-Product]
(
    ID_CProduct int identity primary key,
	[Country_ID] int NOT NULL, --foreign x
	[Product_ID] int not null --foreign x
);
go

-- Credit card storage
CREATE TABLE [CreditCard]
(
    ID_CreditCard int identity primary key,
	Customer_ID int NOT NULL, --foreign x
	[Provider] nvarchar(50) NOT NULL,
	[Number] nvarchar(16) not null,
);
go
-- User
CREATE TABLE [dbo].[User](
  [Id] [int] IDENTITY(1,1) NOT NULL,
  [Username] [nvarchar](50) NOT NULL,
  [PwdHash] [nvarchar](256) NOT NULL,
  [PwdSalt] [nvarchar](256) NOT NULL,
  [FirstName] [nvarchar](256) NOT NULL,
  [LastName] [nvarchar](256) NOT NULL,
  [Email] [nvarchar](256) NOT NULL,
  [Phone] [nvarchar](256) NULL
  CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED (
    [Id] ASC
  )
)
go
-- User related
CREATE TABLE UserRole (
	Id int NOT NULL IDENTITY (1, 1),
	[Name] nvarchar(50) NOT NULL,
	CONSTRAINT PK_UserRole PRIMARY KEY (Id)
)
GO

SET IDENTITY_INSERT UserRole ON
GO

-- User inserts
INSERT INTO UserRole (Id, [Name])
VALUES 
	(1, 'Admin'), 
	(2, 'User')
GO

SET IDENTITY_INSERT UserRole OFF
GO

ALTER TABLE [USER] 
ADD RoleId int NULL
GO

UPDATE [USER]
SET RoleId = 2
GO

ALTER TABLE [USER] 
ALTER COLUMN RoleId int NOT NULL
GO

ALTER TABLE dbo.[USER] 
ADD CONSTRAINT FK_USER_UserRole FOREIGN KEY (RoleId) 
REFERENCES dbo.UserRole (Id)
go

---end of user related

ALTER TABLE [Product]
add foreign key (Category_ID) references Category(ID_Category)
go

ALTER TABLE [Order]
add foreign key (PaymentMethod_ID) references PaymentMethod(ID_PaymentMethod)
go

ALTER TABLE [Order]
add foreign key (Customer_ID) references Customer(ID_Customer)
go

ALTER TABLE [Product-Order]
add foreign key (Order_ID) references [Order](ID_Order);
go

ALTER TABLE [Product-Order]
add foreign key (Product_ID) references Product(ID_Product);
go

ALTER TABLE [Country-Product]
add foreign key (Country_ID) references Country(ID_Country);
go

ALTER TABLE [Country-Product]
add foreign key (Product_ID) references Product(ID_Product);
go

ALTER TABLE [CreditCard]
add foreign key (Customer_ID) references Customer(ID_Customer);
go

print 'Script executed'
go

----- Inserts
insert into Category (Name)
values 
('Graphics cards'),
('Games'),
('Monitors'),
('Cars')
go

insert into Country (Name)
values 
('Croatia'),
('Germany'),
('Slovenia'),
('Italy')
go

insert into Customer(Name, Surname)
values 
('Walter','White'),
('Jesse','Pinkman'),
('Hank','Schrader')
go

insert into CreditCard(Customer_ID, Provider, Number)
values 
(1,'Blue card','4444555566667777')
go

insert into Product (Name, Description, Category_ID, Price, Img_URL, IsAvailable)
values
('GTX 780', 'A powerful graphics processing unit', 1, 499.99,'https://www.techpowerup.com/img/13-06-11/GTX780-DC2OC-3GD5_2D.jpg', 1),
('GTX 1060', 'A powerful graphics processing unit', 1, 299.99,'https://techgage.com/wp-content/uploads/2016/07/ASUS-GeForce-GTX-1060-Strix-Edition.jpg', 1),
('RTX 3060', 'A powerful graphics processing unit', 1, 799.99,'https://pisces.bbystatic.com/image2/BestBuy_US/images/products/6452/6452940_sd.jpg', 1),
('ASUS VG248QE', 'A modern high refresh rate monitor', 3, 499.99,'https://www.flatpanelshd.com/pictures/asusvg248qe-1l.jpg', 1),
('Nissan Skyline R32 1992', 'JDM car', 4, 59999.99,'https://roa.h-cdn.co/assets/16/18/1024x512/gallery-1462289981-gt-r.jpg', 1),
('RTX 4090', 'A powerful graphics processing unit', 1, 2999.99,'https://www.nabava.net/slike/products/41/16/43601641/asus-rog-strix-rtx4090-o24g-gaming-crossfire-24gb-ddr6x_a23a9222.jpeg', 1),
('ASUS ROG Swift PG279Q', 'A modern high refresh rate monitor', 3, 2999.99,'https://www.cclonline.com/images/avante/PG279Q.jpg', 1),
('Nissan Skyline R34 1999', 'JDM car', 4, 159999.99,'https://richmonds.com.au/wp-content/uploads/2020/02/Nissan-Skyline-GTR-R34-blue-1.jpg', 1),
('Call of Duty: Black Ops (2010)', 'A modern shooter', 2, 59.99,'https://m.media-amazon.com/images/M/MV5BYjI1ODZlMjctOTdmMi00NTA3LWJjMzYtOGU5YzBlYzViY2Q2XkEyXkFqcGdeQXVyNjc0NTEzOTA@._V1_.jpg', 1),
('Call of Duty: Black Ops (2010) 2', 'A modern shooter', 2, 59.99,'https://m.media-amazon.com/images/M/MV5BYjI1ODZlMjctOTdmMi00NTA3LWJjMzYtOGU5YzBlYzViY2Q2XkEyXkFqcGdeQXVyNjc0NTEzOTA@._V1_.jpg', 1),
('Call of Duty: Black Ops (2010) 3', 'A modern shooter', 2, 59.99,'https://m.media-amazon.com/images/M/MV5BYjI1ODZlMjctOTdmMi00NTA3LWJjMzYtOGU5YzBlYzViY2Q2XkEyXkFqcGdeQXVyNjc0NTEzOTA@._V1_.jpg', 1),
('Call of Duty: Black Ops (2010) 4', 'A modern shooter', 2, 59.99,'https://m.media-amazon.com/images/M/MV5BYjI1ODZlMjctOTdmMi00NTA3LWJjMzYtOGU5YzBlYzViY2Q2XkEyXkFqcGdeQXVyNjc0NTEzOTA@._V1_.jpg', 1),
('Call of Duty: Black Ops (2010) 5', 'A modern shooter', 2, 59.99,'https://m.media-amazon.com/images/M/MV5BYjI1ODZlMjctOTdmMi00NTA3LWJjMzYtOGU5YzBlYzViY2Q2XkEyXkFqcGdeQXVyNjc0NTEzOTA@._V1_.jpg', 1),
('Call of Duty: Black Ops (2010) 6', 'A modern shooter', 2, 59.99,'https://m.media-amazon.com/images/M/MV5BYjI1ODZlMjctOTdmMi00NTA3LWJjMzYtOGU5YzBlYzViY2Q2XkEyXkFqcGdeQXVyNjc0NTEzOTA@._V1_.jpg', 1),
('Call of Duty: Black Ops (2010) 7', 'A modern shooter', 2, 59.99,'https://m.media-amazon.com/images/M/MV5BYjI1ODZlMjctOTdmMi00NTA3LWJjMzYtOGU5YzBlYzViY2Q2XkEyXkFqcGdeQXVyNjc0NTEzOTA@._V1_.jpg', 1),
('Call of Duty: Black Ops (2010) 8', 'A modern shooter', 2, 59.99,'https://m.media-amazon.com/images/M/MV5BYjI1ODZlMjctOTdmMi00NTA3LWJjMzYtOGU5YzBlYzViY2Q2XkEyXkFqcGdeQXVyNjc0NTEzOTA@._V1_.jpg', 1),
('Call of Duty: Black Ops (2010) 9', 'A modern shooter', 2, 59.99,'https://m.media-amazon.com/images/M/MV5BYjI1ODZlMjctOTdmMi00NTA3LWJjMzYtOGU5YzBlYzViY2Q2XkEyXkFqcGdeQXVyNjc0NTEzOTA@._V1_.jpg', 1),
('Call of Duty: Black Ops (2010) 10', 'A modern shooter', 2, 59.99,'https://m.media-amazon.com/images/M/MV5BYjI1ODZlMjctOTdmMi00NTA3LWJjMzYtOGU5YzBlYzViY2Q2XkEyXkFqcGdeQXVyNjc0NTEzOTA@._V1_.jpg', 1)
go

insert into PaymentMethod (Name)
values
('Cash'),
('CreditCard')
go

insert into [Order] (Customer_ID, PaymentMethod_ID)
values
(1, 2),
(2, 1)
go

insert into [Product-Order] (Order_ID, Product_ID, Quantity)
values
(1,1,1),
(1,2,1),
(1,3,1),
(2,4,1),
(2,5,1)
go

insert into [Country-Product] (Country_ID, Product_ID)
values
(1,1),
(2,1),
(3,1),
(4,1),

(2,2),
(1,2),

(3,3),

(1,4),
(2,4),
(3,4),
(4,4),

(1,5),

(1,6),
(2,6),


(2,7),
(3,7),

(1,8),

(1,9),
(2,9),
(3,9),

(1,10),
(2,10),
(3,10),

(1,11),
(2,11),
(3,11),

(1,12),
(2,12),
(3,12),

(1,13),
(2,13),
(3,13),

(1,14),
(2,14),
(3,14),

(1,15),
(2,15),
(3,15),

(1,16),
(2,16),
(3,16),

(1,17),
(2,17),
(3,17),

(1,18),
(2,18),
(3,18)
go

-- username		password

-- testuser     testtest
-- admin		adminadmin

INSERT INTO [dbo].[User]
           ([Username]
           ,[PwdHash]
           ,[PwdSalt]
           ,[FirstName]
           ,[LastName]
           ,[Email]
           ,[Phone]
           ,[RoleId])
VALUES
	('testuser','PTiJ9wdmzrdsbIi/+Pv2S3C3DUw/xZ8JWPOudL8usWw=','zC40yEy6OTs8xapEX48XRQ==','tester','tester','test@test.local','11111',2),
	('admin','9uGrbFSNZMKetT6jaPl5GBVx5Kdf3YO4hGrfvTUTYUM=','sj6ZD69yP7kG2FE81TuWbw==','Admin','Administrator','admin@admin.local','111222333',1)
GO


print 'Script insertions executed'
go