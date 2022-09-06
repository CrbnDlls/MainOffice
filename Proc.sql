-- =============================================
-- Author:		Corban
-- Create date: 11/03/2020
-- Description:	Updates Employees from DelayedUpdateEmployees
-- =============================================
CREATE PROCEDURE dbo.DelayedUpdateEmployees_proc 
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE	@UpdateTable TABLE
        (
            Id int,
            FamilyName nvarchar(50),
            Name nvarchar(50),
            FathersName nvarchar(50),
            BirthDay date,
            OldFamilyName nvarchar(50),
			ProfessionId int,
			BarberLevelId int,
			SalonId int,
			HireDate date,
			StaffNumber int,
			PhoneNumber1 nvarchar(15),
			PhoneNumber2 nvarchar(15),
			CompanyId int,
			RegisterDate date,
			DismissalDate date
        );
	DECLARE @Count integer;
    -- Insert statements for procedure here
	INSERT INTO
			@UpdateTable
	SELECT	EmployeeId, 
			FamilyName,
            Name,
            FathersName,
            BirthDay,
            OldFamilyName,
			ProfessionId,
			BarberLevelId,
			SalonId,
			HireDate,
			StaffNumber,
			PhoneNumber1,
			PhoneNumber2,
			CompanyId,
			RegisterDate,
			DismissalDate
	FROM
			DelayedUpdateEmployees
	WHERE
			DATEDIFF(day, GETDATE(),UpdateDate) <= 0
			AND EmployeeId IS NOT NULL
	SET @Count = @@ROWCOUNT;

	IF (@Count > 0)
	BEGIN

	UPDATE e
    SET e.FamilyName = u.FamilyName,
		e.Name = u.Name,
		e.FathersName = u.FathersName,
		e.BirthDay = u.BirthDay,
		e.OldFamilyName = u.OldFamilyName,
		e.ProfessionId = u.ProfessionId,
		e.BarberLevelId = u.BarberLevelId,
		e.SalonId = u.SalonId,
		e.HireDate = u.HireDate,
		e.StaffNumber = u.StaffNumber,
		e.PhoneNumber1 = u.PhoneNumber1,
		e.PhoneNumber2 = u.PhoneNumber2,
		e.CompanyId = u.CompanyId,
		e.RegisterDate = u.DismissalDate
    FROM Employees e
    JOIN @UpdateTable u
    ON e.Id = u.Id;
	
	DELETE FROM @UpdateTable;

	END
	
	INSERT INTO
			@UpdateTable
	SELECT	EmployeeId, 
			FamilyName,
            Name,
            FathersName,
            BirthDay,
            OldFamilyName,
			ProfessionId,
			BarberLevelId,
			SalonId,
			HireDate,
			StaffNumber,
			PhoneNumber1,
			PhoneNumber2,
			CompanyId,
			RegisterDate,
			DismissalDate
	FROM
			DelayedUpdateEmployees
	WHERE
			DATEDIFF(day, GETDATE(),UpdateDate) <= 0
			AND EmployeeId IS NULL
	SET @Count = @@ROWCOUNT;

	IF (@Count > 0)
	BEGIN
	
	INSERT INTO Employees(FamilyName,
            Name,
            FathersName,
            BirthDay,
            OldFamilyName,
			ProfessionId,
			BarberLevelId,
			SalonId,
			HireDate,
			StaffNumber,
			PhoneNumber1,
			PhoneNumber2,
			CompanyId,
			RegisterDate,
			DismissalDate)
	SELECT FamilyName,
            Name,
            FathersName,
            BirthDay,
            OldFamilyName,
			ProfessionId,
			BarberLevelId,
			SalonId,
			HireDate,
			StaffNumber,
			PhoneNumber1,
			PhoneNumber2,
			CompanyId,
			RegisterDate,
			DismissalDate 
	FROM @UpdateTable;
	END
	DELETE FROM DelayedUpdateEmployees
	WHERE
			DATEDIFF(day, GETDATE(),UpdateDate) <= 0;
END