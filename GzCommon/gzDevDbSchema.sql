    USE master ;
    GO
    CREATE DATABASE gzDevDb
    ON 
    ( NAME = gzDevDb,
        FILENAME = 'D:\sc\gz\GzCommon\gzDevDb.mdf',
        SIZE = 10,
        MAXSIZE = 50,
        FILEGROWTH = 5 )
    LOG ON
    ( NAME = gzDevDb_log,
        FILENAME = 'D:\sc\gz\GzCommon\gzDevDb.ldf',
        SIZE = 5MB,
        MAXSIZE = 25MB,
        FILEGROWTH = 5MB ) ;
    GO
