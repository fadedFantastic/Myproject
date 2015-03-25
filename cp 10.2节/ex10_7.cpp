#include <iostream>
#include <vector>
#include <list>
#include <iterator>

using namespace std;

template<typename sequence>
void print(sequence &seq)
{
	for (auto it = seq.begin(); it != seq.end(); ++it)
	{
		cout << *it << " ";
	}
	cout << endl;
}

int main()
{
	//(a) 
	vector<int> vec;
	list<int> lst;
	int i;
	while (cin >> i)
		lst.push_back(i);
	copy(lst.cbegin(), lst.cend(), back_inserter(vec));  
	
	//(b)
	vector<int> v;
	v.resize(10);
	fill_n(v.begin(), 10, 0);

	print(vec);
	print(v);
	return 0;
}