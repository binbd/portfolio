using System.Collections.Concurrent;

public static class Config{
       public static ConcurrentDictionary<string,object> Application = new ConcurrentDictionary<string,object>();
       public static ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
}