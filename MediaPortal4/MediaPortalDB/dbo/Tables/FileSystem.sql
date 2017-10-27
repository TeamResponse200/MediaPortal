CREATE TABLE [dbo].[FileSystem] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [UserId]        NVARCHAR (128) NOT NULL,
    [ParentId]      INT            NULL,
    [Name]          NVARCHAR (256) NOT NULL,
    [Size]          INT            NULL,
    [Type]          NVARCHAR (256) NOT NULL,
    [BlobLink]      NVARCHAR (256) NULL,
    [BlobThumbnail] NVARCHAR (1)   NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([ParentId]) REFERENCES [dbo].[FileSystem] ([Id]),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);

