namespace VinValidator.Core.Filter;

public class GroupFilter
{
    private List<Predicate<object>> filters;

    public Predicate<object> Filter { get; private set; }

    public GroupFilter()
    {
        filters = [];
        Filter = InternalFilter;
    }

    private bool InternalFilter(object o)
    {
        foreach (var filter in filters)
            if (!filter(o))
                return false;
        return true;
    }

    public void AddFilter(Predicate<object> filter)
        => filters.Add(filter);

    public void RemoveFilter(Predicate<object> filter)
        => filters.Remove(filter);
}
