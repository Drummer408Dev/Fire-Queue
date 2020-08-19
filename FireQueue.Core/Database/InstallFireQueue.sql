IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Queues')
BEGIN
    CREATE TABLE [Queues] (
        [QueueId] [int] IDENTITY(1, 1) NOT NULL,
        [Name] [varchar] (128) UNIQUE NOT NULL,
        [DateCreated] [datetime] NOT NULL
    )
END


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Messages')
BEGIN
    CREATE TABLE [Messages] (
        [MessageId] [int] IDENTITY(1, 1) NOT NULL,
        [Contents] [varchar] (max),
        [QueueId] [int] NOT NULL,
        [DatePublished] [datetime] NOT NULL
    )
END