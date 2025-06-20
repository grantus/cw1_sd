namespace cw1_sd;

public enum CategoryType
{
    Income,
    Expense
}

public class CategoryFacade
{
    private static int _nextId;
    private static readonly Dictionary<int, Category> CategoryRepository = new();

    public static void LoadFromList(List<Category> categories)
    {
        CategoryRepository.Clear();
        _nextId = 0;

        foreach (var category in categories)
        {
            if (category.Id >= _nextId)
            {
                _nextId = category.Id + 1;
            }

            CategoryRepository[category.Id] = category;
        }
    }

    public static List<Category> ToList()
    {
        return CategoryRepository.Values.ToList();
    }

    public void CreateCategory(string name, CategoryType type)
    {
        var category = new Category { Id = _nextId++, Name = name, Type = type };
        CategoryRepository[category.Id] = category;
    }

    public static void DeleteCategory(int id)
    {
        CategoryRepository.Remove(id);
    }

    public static Category GetCategoryStatic(int id, Dictionary<int, Category> repo)
    {
        return repo[id];
    }

    public static Category? GetCategory(int id)
    {
        if (CategoryRepository.ContainsKey(id))
        {
            return CategoryRepository[id];
        }
        else
        {
            return null;
        }
    }

    public static int EditCategory(int id, string? name = null, CategoryType? type = null)
    {
        var category = GetCategory(id);
        if (category == null)
        {
            return -1;
        }

        if (name != null)
        {
            category.Name = name;
        }

        if (type == null)
        {
            return 0;
        }

        category.Type = type.Value;
        return 0;
    }
}

public class Category
{
    public int Id { get; init; }
    public CategoryType Type { get; set; }
    public string Name { get; set; } = string.Empty;
}