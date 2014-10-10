#include "Sales_data.h"
#include <iostream>

using std::cin;  using std::cout;
using std::endl; using std::cerr;
 
int main()
{
	Sales_data book1, book2;
	if (cin >> book1.bookNo >> book1.units_sold >> book1.revenue){
		while (cin >> book2.bookNo >> book2.units_sold >> book2.revenue){
			if (book1.bookNo == book2.bookNo){
				book1.units_sold = book1.units_sold + book2.units_sold;
			}
			else{
				cout << book1.bookNo << " " << book1.units_sold << endl;
				book1 = book2;
			}
		}
		cout << book1.bookNo << " " << book1.units_sold << endl;
	}
	else{
		cerr << "no data ?!" << endl;
	}
}