using PlanningExperimentsLabs.Laboratories._1;
using PlanningExperimentsLabs.Laboratories._2;
using PlanningExperimentsLabs.Laboratories._3;
using PlanningExperimentsLabs.Laboratories._4;

namespace PlanningExperimentsLabs;

public static class Program
{
    private const int InstanceCount = 150000;

    public static void Main()
    {
        FourthLaboratory.Start(InstanceCount);
    }
}