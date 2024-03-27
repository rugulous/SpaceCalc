using Directory = SpaceCalc.Directory;

List<Directory> GetFoldersAndSize(DirectoryInfo directory, List<Directory> data, string currPath = "")
{
    long length = 0;
    if (string.IsNullOrWhiteSpace(currPath))
    {
        currPath = directory.Name;
    }
    else
    {
        currPath = $"{currPath}/{directory.Name}";
    }

    try
    {
        foreach (FileInfo file in directory.GetFiles())
        {
            length += file.Length;
        }
    }
    catch
    {
        Console.WriteLine($"Failed to read files in {currPath}");
    }

    data.Add(new (currPath, length));

    try
    {
        foreach (DirectoryInfo subDir in directory.GetDirectories())
        {
            data.AddRange(GetFoldersAndSize(subDir, new(), currPath));
        }
    }
    catch
    {
        Console.WriteLine($"Failed to read directories in {currPath}");
    }

    return data;
}

string BytesToString(long byteCount)
{
    string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
    if (byteCount == 0)
    {
        return "0" + suf[0];
    }

    long bytes = Math.Abs(byteCount);
    int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
    double num = Math.Round(bytes / Math.Pow(1024, place), 1);

    return (Math.Sign(byteCount) * num).ToString() + suf[place];
}

DirectoryInfo root = new("C:\\");
List<Directory> folders = GetFoldersAndSize(root, new());

Console.WriteLine();
Console.WriteLine($"Found {folders.Count} folders");

folders.Sort((a, b) =>
{
    if(a.Bytes == b.Bytes)
    {
        return 0;
    }

    return a.Bytes > b.Bytes ? -1 : 1;
});

int size = Math.Min(folders.Count, 100);
Console.WriteLine($"Top {size} largest folders");

for(int i = 0; i < size; i++)
{
    Console.WriteLine($"[{i + 1}]. {folders[i].FullPath} - {BytesToString(folders[i].Bytes)}");
}