#include <iostream>
#include <vector>
#include <numeric>

using namespace std;

//! Exercise 10.3
int add_int(const vector<int> &v)
{
	int sum = accumulate(v.cbegin(), v.cend(), 0);
	return sum;
}

int main()
{
	//! Exercise 10.3
	vector<int> vec = { 1, 2, 3, 4, 5, 6 };
	cout << add_int(vec) << endl;


	//! Exercise 10.4
	vector<double> vecd = {1.1, 2.0, 3.3};
	cout << accumulate(vecd.cbegin(), vecd.cend(), 0) << endl;


	return 0;
}