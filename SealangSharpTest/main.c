void main(int argc, char **argv)
{
	int a = 5;
	int b = 6;
	float f = 3.4;
	char c[] = "Hello, World!";

	if (a < b || b >= f) {
		a += b;
	}
	else {
		a++;
		--b;

		c[a] = 'x';
	}

	return a << b;
}