#include "Sales_data.h"
#include <iostream>

using std::istream;
using std::ostream;

istream &read(istream &is, Sales_data &item)
{
	double price = 0;
	is >> item.bookNo >> item.units_sold >> price;
	item.revenue = price * item.units_sold;
	return is;
}

ostream &print(ostream &os, const Sales_data &item)
{
	os << item.isbn() << " " << item.units_sold << " "
	   << item.revenue;
	return os;
}

Sales_data add(const Sales_data &lhs, const Sales_data&rhs)
{
	Sales_data sum = lhs;
	sum.combine(rhs);
	return sum;
}

Sales_data::Sales_data(istream &is)
{
	read(is, *this);
}
inline
double Sales_data::avg_price() const
{
	return units_sold ? revenue / units_sold : 0;
}


#include <string>
using std::string;

int main()
{
	string s("99-9999999-99");
	Sales_data i;
	i.combine(s);
}