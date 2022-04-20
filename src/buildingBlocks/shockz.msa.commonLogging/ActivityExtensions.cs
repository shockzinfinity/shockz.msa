using System.Diagnostics;

namespace shockz.msa.commonLogging;

public static class ActivityExtensions
{
  public static string GetTraceId(this Activity activity)
  {
    if (activity == null) return string.Empty;

    return activity.IdFormat switch
    {
      ActivityIdFormat.Hierarchical => activity.RootId,
      ActivityIdFormat.W3C => activity.TraceId.ToString(),
      _ => null,
    } ?? string.Empty;
  }

  public static string GetSpanId(this Activity activity)
  {
    if (activity == null) return string.Empty;

    return activity.IdFormat switch
    {
      ActivityIdFormat.Hierarchical => activity.Id,
      ActivityIdFormat.W3C => activity.SpanId.ToString(),
      _ => null
    } ?? string.Empty;
  }

  public static string GetParentId(this Activity activity)
  {
    if (activity == null) return string.Empty;

    return activity.IdFormat switch
    {
      ActivityIdFormat.Hierarchical => activity.ParentId,
      ActivityIdFormat.W3C => activity.ParentSpanId.ToString(),
      _ => null
    } ?? string.Empty;
  }
}
