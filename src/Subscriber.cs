using design_patterns.ProblemManagement;

public abstract class Subscriber {
    protected Problem problem;
    protected ProblemUpdateVisitor visitor;
    
    public void Poke(){
        foreach (var update in problem.Updates.Keys){
            // Console.WriteLine(update == null ? "Poked with update: null" : "Poked with update: " + update.ToString());
            if (update.Source == this){
                continue;
            }
            bool isAcknowledged = update.Acknowledge(this);
            if (isAcknowledged){
                continue;
            }
            update.Accept(visitor);
        }
    }
}