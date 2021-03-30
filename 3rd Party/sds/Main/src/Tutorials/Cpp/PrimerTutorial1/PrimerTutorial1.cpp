#include"SDS.h"

using namespace sds;

using namespace Microsoft::Research::Science::Data;
using namespace Microsoft::Research::Science::Data::Imperative;

int main(int argc, const char **argv)
{
	// read input data
	CppDataSet dataset("Tutorial1.csv");
	int *xshape;
	double *x = dataset.GetDoubles("X",&xshape);
	int *yshape;
	double *y = dataset.GetDoubles("Observation",&yshape);

	// ylen[0] == xlen[0] because X and Y are defined on the same dimension
	int len = xshape[0]; 

	// compute
    double xm = 0;
	for(int i =0;i<len;i++)
		xm += x[i];
	xm /= len;

    double ym = 0;
	for(int i =0;i<len;i++)
		ym += y[i];
	ym /= len;

	double a = 0;
	double d = 0;
	for(int i = 0;i<len;i++) 
	{
		a += (x[i] - xm) * (y[i] - ym);
		d += (x[i] - xm) * (x[i] - xm);
	}
	a /= d;
	double b = ym - a * xm;

	double *model = new double[len];
	for(int i = 0;i<len;i++)
		model[i] = a * x[i] + b;

	dataset.Add(DataType::DOUBLE, "Model", "_1");
	dataset.PutDoubles("Model", model, xshape);

	// Cleanup
	delete[] xshape;
	delete[] yshape;
	delete[] x;
	delete[] y;
	delete[] model;
}