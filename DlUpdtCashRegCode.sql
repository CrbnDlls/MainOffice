-- =============================================
-- Author:		Corban
-- Create date: 11/03/2020
-- Description:	Updates CashRegCodes from DelayedUpdateCashRegCodes
-- =============================================
CREATE PROCEDURE [dbo].[DelayedUpdateCashRegCode_proc] 
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE	@UpdateTable TABLE
        (
            Id int,
            Code int,
			PriceListUnitId int,
			ProductId int,
			ServiceId int,
			Price decimal,
			Price10 decimal,
			Price50 decimal,
			PriceStaff decimal
		);
	DECLARE @Count integer;
    -- Insert statements for procedure here
	INSERT INTO
			@UpdateTable
	SELECT	CashRegCodeId,
            Code,
			PriceListUnitId,
			ProductId,
			ServiceId,
			Price,
			Price10,
			Price50,
			PriceStaff
	FROM
			DelayedUpdateCashRegCodes
	WHERE
			DATEDIFF(day, GETDATE(),UpdateDate) <= 0
			AND CashRegCodeId IS NOT NULL
	SET @Count = @@ROWCOUNT;

	IF (@Count > 0)
	BEGIN

	UPDATE e
    SET e.Code = u.Code,
		e.PriceListUnitId = u.PriceListUnitId,
		e.ProductId = u.ProductId,
		e.ServiceId = u.ServiceId,
		e.Price = u.Price,
		e.Price10 = u.Price10,
		e.Price50 = u.Price50,
		e.PriceStaff = u.PriceStaff
    FROM CashRegCodes e
    JOIN @UpdateTable u
    ON e.Id = u.Id;
	
	DELETE FROM @UpdateTable;

	END
	
	INSERT INTO
			@UpdateTable
	SELECT	CashRegCodeId,
            Code,
			PriceListUnitId,
			ProductId,
			ServiceId,
			Price,
			Price10,
			Price50,
			PriceStaff
	FROM
			DelayedUpdateCashRegCodes
	WHERE
			DATEDIFF(day, GETDATE(),UpdateDate) <= 0
			AND CashRegCodeId IS NULL
	SET @Count = @@ROWCOUNT;

	IF (@Count > 0)
	BEGIN
	
	INSERT INTO CashRegCodes(Code,
			PriceListUnitId,
			ProductId,
			ServiceId,
			Price,
			Price10,
			Price50,
			PriceStaff)
	SELECT Code,
			PriceListUnitId,
			ProductId,
			ServiceId,
			Price,
			Price10,
			Price50,
			PriceStaff 
	FROM @UpdateTable;
	END
	DELETE FROM DelayedUpdateCashRegCodes
	WHERE
			DATEDIFF(day, GETDATE(),UpdateDate) <= 0;
END