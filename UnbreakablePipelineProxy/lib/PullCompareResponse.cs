using System.Collections.Generic;

public class PullCompareResponse {
    public List<PerformanceSignature> performanceSignature {get;set;}
    public int totalViolations {get;set;}
    public string comment {get;set;}
}