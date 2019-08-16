enum FileMode {
	CreateNew = 1,
	Create,
	Open,
	OpenOrCreate,
	Truncate,
	Append
}

enum FileAccess {
	Read = 1,
	Write,
	ReadWrite
}

def main(args : [string]) -> void {
	handle := openFile("a.txt", FileMode.OpenOrCreate, FileAccess.Write);	
	defer(closeFile(handle));
	writeToFile(handle, "hello world");

}