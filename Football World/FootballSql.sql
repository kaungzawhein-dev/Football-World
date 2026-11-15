CREATE DATABASE FootballWorldDB;
GO

USE FootballWorldDB;
GO
CREATE TABLE Leagues
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Country NVARCHAR(100) NOT NULL,
    LogoUrl NVARCHAR(500) NULL
);
GO

CREATE TABLE Teams
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    LeagueId INT NOT NULL,
    Name NVARCHAR(150) NOT NULL,
    Country NVARCHAR(100) NOT NULL,
    FoundedYear INT NULL,
    Stadium NVARCHAR(150) NULL,
    Manager NVARCHAR(150) NULL,
    LogoUrl NVARCHAR(500) NULL,
    Description NVARCHAR(MAX) NULL,

    CONSTRAINT FK_Teams_Leagues
        FOREIGN KEY (LeagueId)
        REFERENCES Leagues(Id)
        ON DELETE CASCADE
);
GO

CREATE TABLE Players
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TeamId INT NOT NULL,
    Name NVARCHAR(150) NOT NULL,
    Position NVARCHAR(50) NOT NULL,
    ShirtNumber INT NULL,
    Nationality NVARCHAR(100) NULL,
    DateOfBirth DATE NULL,
    HeightCm INT NULL,
    PhotoUrl NVARCHAR(500) NULL,

    CONSTRAINT FK_Players_Teams
        FOREIGN KEY (TeamId)
        REFERENCES Teams(Id)
        ON DELETE CASCADE
);
GO

CREATE TABLE Matches
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    LeagueId INT NOT NULL,
    HomeTeamId INT NOT NULL,
    AwayTeamId INT NOT NULL,
    MatchDate DATETIME2 NOT NULL,
    Venue NVARCHAR(150) NULL,
    HomeScore INT NULL,
    AwayScore INT NULL,
    Status NVARCHAR(30) NOT NULL DEFAULT 'Scheduled',

    CONSTRAINT FK_Matches_Leagues
        FOREIGN KEY (LeagueId) REFERENCES Leagues(Id),

    CONSTRAINT FK_Matches_HomeTeam
        FOREIGN KEY (HomeTeamId) REFERENCES Teams(Id),

    CONSTRAINT FK_Matches_AwayTeam
        FOREIGN KEY (AwayTeamId) REFERENCES Teams(Id),

    CONSTRAINT CK_Matches_DifferentTeams
        CHECK (HomeTeamId <> AwayTeamId),

    CONSTRAINT CK_Matches_Scores
        CHECK (
            (HomeScore IS NULL OR HomeScore >= 0)
            AND (AwayScore IS NULL OR AwayScore >= 0)
        )
);
GO


CREATE TABLE Lineups
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MatchId INT NOT NULL,
    TeamId INT NOT NULL,
    PlayerId INT NOT NULL,
    IsStarting BIT NOT NULL DEFAULT 1,
    ShirtNumber INT NULL,
    Position NVARCHAR(50) NULL,

    CONSTRAINT FK_Lineups_Matches
        FOREIGN KEY (MatchId) REFERENCES Matches(Id),

    CONSTRAINT FK_Lineups_Teams
        FOREIGN KEY (TeamId) REFERENCES Teams(Id),

    CONSTRAINT FK_Lineups_Players
        FOREIGN KEY (PlayerId) REFERENCES Players(Id),

    CONSTRAINT UQ_Lineups_MatchPlayer
        UNIQUE (MatchId, PlayerId)
);
GO

CREATE TABLE Standings
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    LeagueId INT NOT NULL,
    TeamId INT NOT NULL,
    Season NVARCHAR(20) NOT NULL,
    Played INT NOT NULL DEFAULT 0,
    Won INT NOT NULL DEFAULT 0,
    Drawn INT NOT NULL DEFAULT 0,
    Lost INT NOT NULL DEFAULT 0,
    GoalsFor INT NOT NULL DEFAULT 0,
    GoalsAgainst INT NOT NULL DEFAULT 0,
    GoalDifference INT NOT NULL DEFAULT 0,
    Points INT NOT NULL DEFAULT 0,

    CONSTRAINT FK_Standings_Leagues
        FOREIGN KEY (LeagueId) REFERENCES Leagues(Id),

    CONSTRAINT FK_Standings_Teams
        FOREIGN KEY (TeamId) REFERENCES Teams(Id),

    CONSTRAINT UQ_Standings_LeagueTeamSeason
        UNIQUE (LeagueId, TeamId, Season)
);
GO

CREATE TABLE Trophies
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TeamId INT NOT NULL,
    Name NVARCHAR(150) NOT NULL,
    Competition NVARCHAR(150) NULL,
    Season NVARCHAR(20) NULL,
    Year INT NULL,
    Description NVARCHAR(MAX) NULL,

    CONSTRAINT FK_Trophies_Teams
        FOREIGN KEY (TeamId)
        REFERENCES Teams(Id)
        ON DELETE CASCADE
);
GO

CREATE INDEX IX_Teams_LeagueId
ON Teams(LeagueId);

CREATE INDEX IX_Players_TeamId
ON Players(TeamId);

CREATE INDEX IX_Matches_LeagueId
ON Matches(LeagueId);

CREATE INDEX IX_Matches_MatchDate
ON Matches(MatchDate);

CREATE INDEX IX_Standings_LeagueSeason
ON Standings(LeagueId, Season);
GO

INSERT INTO Leagues (Name, Country, LogoUrl)
VALUES
('Premier League', 'England', NULL),
('La Liga', 'Spain', NULL),
('Serie A', 'Italy', NULL),
('Bundesliga', 'Germany', NULL),
('Ligue 1', 'France', NULL);
GO

INSERT INTO Teams
(LeagueId, Name, Country, FoundedYear, Stadium)
VALUES
(1, 'Manchester United', 'England', 1878, 'Old Trafford'),
(1, 'Liverpool', 'England', 1892, 'Anfield'),
(1, 'Arsenal', 'England', 1886, 'Emirates Stadium'),
(2, 'Real Madrid', 'Spain', 1902, 'Santiago Bernabeu'),
(2, 'Barcelona', 'Spain', 1899, 'Spotify Camp Nou');
GO