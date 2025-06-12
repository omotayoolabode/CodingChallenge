BEGIN TRANSACTION;
GO

CREATE TABLE [Journeys] (
    [JourneyId] int NOT NULL IDENTITY,
    [Origin] nvarchar(1) NOT NULL,
    [Destination] nvarchar(1) NOT NULL,
    [DepartureTime] datetime2 NULL,
    CONSTRAINT [PK_Journeys] PRIMARY KEY ([JourneyId])
    );
GO

CREATE TABLE [Routes] (
    [RouteId] nvarchar(50) NOT NULL,
    [From] nvarchar(1) NOT NULL,
    [To] nvarchar(1) NOT NULL,
    [Duration] int NOT NULL,
    CONSTRAINT [PK_Routes] PRIMARY KEY ([RouteId])
    );
GO

CREATE TABLE [Flights] (
    [FlightId] int NOT NULL IDENTITY,
    [RouteId] nvarchar(50) NOT NULL,
    [Provider] nvarchar(100) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Departure] datetime2 NOT NULL,
    CONSTRAINT [PK_Flights] PRIMARY KEY ([FlightId]),
    CONSTRAINT [FK_Flights_Routes_RouteId] FOREIGN KEY ([RouteId]) REFERENCES [Routes] ([RouteId]) ON DELETE CASCADE
    );
GO

CREATE TABLE [JourneyFlights] (
    [JourneyFlightId] int NOT NULL IDENTITY,
    [JourneyId] int NOT NULL,
    [FlightId] int NOT NULL,
    [Sequence] int NOT NULL,
     CONSTRAINT [PK_JourneyFlights] PRIMARY KEY ([JourneyFlightId]),
    CONSTRAINT [FK_JourneyFlights_Flights_FlightId] FOREIGN KEY ([FlightId]) REFERENCES [Flights] ([FlightId]) ON DELETE CASCADE,
    CONSTRAINT [FK_JourneyFlights_Journeys_JourneyId] FOREIGN KEY ([JourneyId]) REFERENCES [Journeys] ([JourneyId]) ON DELETE CASCADE
    );
GO

CREATE INDEX [IX_Flights_RouteId] ON [Flights] ([RouteId]);
GO

CREATE INDEX [IX_JourneyFlights_FlightId] ON [JourneyFlights] ([FlightId]);
GO

CREATE INDEX [IX_JourneyFlights_JourneyId] ON [JourneyFlights] ([JourneyId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250612193911_AddedRouteAndFlightData', N'8.0.17');
GO

COMMIT;
GO

