CREATE TABLE [dbo].[FileTag] (
    [Id]           INT IDENTITY (1, 1) NOT NULL,
    [FileSystemId] INT NOT NULL,
    [TagId]        INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([FileSystemId]) REFERENCES [dbo].[FileSystem] ([Id]),
    FOREIGN KEY ([TagId]) REFERENCES [dbo].[Tag] ([Id])
);

