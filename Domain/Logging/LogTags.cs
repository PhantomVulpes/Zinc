﻿namespace Vulpes.Zinc.Domain.Logging;
public static class LogTags
{
    // General tags
    public static string Success => $"[{nameof(Success)}]";
    public static string Warning => $"[{nameof(Warning)}]";
    public static string Failure => $"[{nameof(Failure)}]";
    public static string UnimplementedMethod => $"[{nameof(UnimplementedMethod)}]";

    // Database Tags
    public static string DatabaseOpened => $"[{nameof(DatabaseOpened)}]";
    public static string QueryReport => $"[{nameof(QueryReport)}]";
    public static string EntityDeleted => $"[{nameof(EntityDeleted)}]";
    public static string EntityUpdated => $"[{nameof(EntityUpdated)}]";
    public static string EntityInserted => $"[{nameof(EntityInserted)}]";
}
