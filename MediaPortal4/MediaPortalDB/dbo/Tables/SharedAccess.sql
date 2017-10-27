CREATE TABLE [dbo].[SharedAccess] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [UserId]         NVARCHAR (128) NULL,
    [Link]           NVARCHAR (256) NOT NULL,
    [ExpirationDate] DATETIME       DEFAULT (dateadd(day,(7),getdate())) NOT NULL,
    [FileSystemId]   INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([FileSystemId]) REFERENCES [dbo].[FileSystem] ([Id]),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);

