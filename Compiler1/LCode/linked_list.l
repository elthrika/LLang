struct Node {
	next:Node=null;
	prev:Node=null;
	content:int = 0;
}

struct List {
	start:Node;
	last:Node;
	length:int;
}

def ListMake() -> List {
	return new List;
}

def ListAdd(l:List, content:int) -> void {
	n := new Node;
	n.content = content;
	#if(l.last != null){
	l.last.next = n;
	#}
	n.prev = l.last;
	l.last = n;
	l.length += 1;
}

def ListAt(l:List, idx:int) -> int {
	if(idx > l.length)
		return -1;
	cur := l.start;
	for(i in [0 .. idx]){
		cur = cur.next;
	}
	return cur.content;
}

def main(args:[string]) -> void {
	l := ListMake();
	ListAdd(l, 1);
	l:Add(2);
	l:Add(3);
	e := l:At(0);
	#print(e);
}