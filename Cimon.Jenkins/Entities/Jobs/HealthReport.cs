namespace Cimon.Jenkins.Entities.Jobs;

public record HealthReport
{
    public string Description { get; set; }
    public int Score { get; set; }

}
