CREATE TABLE Vehicle
(
	VehicleKey INT NOT NULL IDENTITY PRIMARY KEY,
	VehicleID varChar(17) NOT NULL,
	Make varChar(50) NOT NULL,
	Model varChar(50) NOT NULL,
	Year smallint NOT NULL,
	OwnerKey INT NULL REFERENCES Owner(OwnerKey)
)
