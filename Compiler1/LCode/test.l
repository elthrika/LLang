struct Node {
	next:Node;
	prev:Node;
	content:int;
}

struct foo {
	i:int;
	j:float;
}

global x := 42;

def atoi(s:string) -> int {
	return 1;
}

def neg(a:int)->int {
	return -a;
}

def abs(a:int)->int {
	if(a < 0){
		return -a;
	} else if(a > 0) {
		return a;
	}
	return x;
}

def fun(a : int, b : int) -> int {
	# Euklidian GCD algorithm
	while(b != 0) {
		t := b;
		b = a % b;
		a = t;
	}
	return a;
}

def main(args : [string]) -> void {
	x := atoi(args[0]);
	a := -(3 + 5) * 4 - 3 / 2 * 1;
	b : int = 3;
	f : foo = new foo;
	
	funs := [abs, neg];
	funs[0](4);
	
	n : Node = new Node;
	r : Node = new Node;
	n.next = null;
	n.prev = r;
	r.next = n;
	r.prev = null;
		
	b = 4;
	for(i in [1,2,3,4,5,6,7]){
		a += i;
	}
	fun(a,b);
	b += f.i + fun(a,b);
	
	return;
}