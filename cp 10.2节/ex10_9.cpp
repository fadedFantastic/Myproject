#include <iostream>
#include <vector>
#include <string>
#include <algorithm>

using namespace std;

template<typename sequence>
void print(sequence const& seq)
{
	for (auto it = seq.begin(); it != seq.end(); ++it)
	{
		cout << *it << " ";
	}
	cout << endl;
}

void elimDups(vector<string> &words)
{
	print(words);

	sort(words.begin(), words.end());
	print(words);

	auto end_unique = unique(words.begin(), words.end());
	print(words);

	words.erase(end_unique, words.end());
	print(words);
}

int main()
{
	vector<string> svec{"the","quick", "red", "fox", "jumps", 
		              "over","the", "slow", "red", "turtle"};
	elimDups(svec);

	return 0;
}