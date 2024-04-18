<Query Kind="Program" />

void Main()
{
	var path = Directory.GetParent(Path.GetDirectoryName(Util.CurrentQueryPath)).FullName;;
	foreach (var file in DirectoryHelper.EnumerateAllNestedDirectories(path, IgnoreNonSourceFolders).SelectMany(x => Directory.GetFiles(x, "*.cs")))
	{
		EscapePreprocessorDirectives(file);
	}
}

// Define other methods and classes here
bool IgnoreNonSourceFolders(string path)
{
	var name = path.Split('\\').Last();
	var ignoredFolders = "bin,obj,doc,lib,library,packages".Split(',');
	
	return !name.StartsWith(".") && !ignoredFolders.Contains(name, StringComparer.InvariantCultureIgnoreCase);
}
bool EscapePreprocessorDirectives(string path)
{
 	if (Path.GetExtension(path) != ".cs")
		throw new NotSupportedException("File type: " + Path.GetExtension(path));
	
	var lines = File.ReadAllLines(path);
	var buffer = new StringBuilder();
	
	var noEmitFlag = false;
	var isModified = false;
	var isInsideOption = false;
	var innerConditionCounter = 0;
	foreach (var line in lines)
	{
		if (RegexExtract(line, @"(\+|-):cnd:noEmit", "1") is string flag)
		{
			noEmitFlag = flag == "-";
		}
		else if (Regex.IsMatch(line, @"^#if\s*[(]+.*$"))
		{
			isInsideOption = true;
			buffer.AppendLine(line);
			continue;
		}
		else if (Regex.IsMatch(line, @"^#(if|else|elif|endif)\s*[^(]*$") && !noEmitFlag)
		{
			if (isInsideOption)
			{
				if (line.Contains("#if"))
				{
					innerConditionCounter++;
				}
				else if (line.Contains("#endif"))
				{
					if (innerConditionCounter > 0)
					{
						innerConditionCounter--;
					}
					else
					{
						isInsideOption = false;
						buffer.AppendLine(line);
						continue;
					}
				}
			}
			
			buffer.AppendLine("//-:cnd:noEmit");
			buffer.AppendLine(line);
			buffer.AppendLine("//+:cnd:noEmit");
			
			isModified = true;
			continue;
		}
		
		buffer.AppendLine(line);
	}
	
	if (isModified)
		File.WriteAllText(path, buffer.ToString());
	
	return isModified;
	
	string RegexExtract(string value, string pattern, string group) => Regex.Match(value, pattern, RegexOptions.Compiled) is var match && match.Success ? match.Groups[group].Value : default;
}

public static class DirectoryHelper
{
	public static IEnumerable<string> EnumerateAllNestedDirectories(string path) => EnumerateAllNestedDirectories(path, _ => true);
	public static IEnumerable<string> EnumerateAllNestedDirectories(string path, Func<string, bool> predicate)
	{
		foreach (var directory in Directory.EnumerateDirectories(path))
		{
			if (predicate(directory))
			{
				yield return directory;
				foreach (var subdirectory in EnumerateAllNestedDirectories(directory, predicate))
				{
					yield return subdirectory;
				}
			}
		}
	}
}