#include <iostream>
#include <vector>

using namespace std;

int main()
{
	vector<int> vec = {1, 2, 3, 4, 5};
	fill_n(vec.begin(), vec.size(), 0);  //������Ԫ������Ϊ0
	for (auto it = vec.begin(); it != vec.end(); ++it)
	{
		cout << *it << " ";
	}
	return 0;
}