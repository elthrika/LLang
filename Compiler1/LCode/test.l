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

def stringAt(s : string, i : int) -> int {
	return s._str[i];
}

def atoi(s:string) -> int {
	num := 0;
	for(n in [0 .. s.length])
	{
		num = (num * 10) + s:At(n) - 48;
	}
	return num;
}

def neg(a:float)->float{
	return -a;
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
	
	f.j = b;

	funs := [abs, neg];
	#print(funs[0](-4));
	
	n : Node = new Node;
	r : Node = new Node;
	n.next = null;
	n.prev = r;
	r.next = n;
	r.prev = null;
	
	fl : float = neg(-1);
	#exit(2);

	b = 4;
	for(i in [1,2,3,4,5,6,7]){
		a += i;
	}
	fun(a,b);
	b += f.i + fun(a,b);
	
	print(x);

	return;
}