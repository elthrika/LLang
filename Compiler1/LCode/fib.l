def fib(n : int) -> int {
	nums := [0, 1, 1];
	for(i in [2..n]) {
		nums[0] = nums[1];
		nums[1] = nums[2];
		nums[2] = nums[0] + nums[1];
	}
	return nums[2];
}

def main(args : [string]) -> void {
	for(i in [1..90]) {
		print(i);
		print(fib(i));
	}
}