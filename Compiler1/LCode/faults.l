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

global x := 33;

def atoi(s:string) -> float {
	return 1;
}

def wtfuntion(f:foo) -> void {
	if(f){
		f.i = 4;
		f.j = 5;
	}
}

def abs(a:int)->int {
	if(a < 0){
		return -a;
	} else {
		return a;
	}
	return 3.14159;
}

def fun(x:int, y:int, z:int) -> int {
	if(x < y) {
		return x;
	} else if(x < z){
		return y;
	} else {
		z += 3;
	}
	return z;
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
	x := atoi(33);
	a := -(3 + 5) * 4.9 - 3 / 2 * 1;
	b : int = 3.14159;
	f : foo = new foo;
	
	n : Node = new Node;
	r : Node = new Node;
	n.next = null;
	n.prev = r;
	r.next = n;
	r.prev = null;
	
	b := 4;
	for(i in [1,2,3,"foo",5,6,7]){
		a += i;
	}
	
	fun(a,b,c);
	b += f.i;
	
	return fun(a,b);
}