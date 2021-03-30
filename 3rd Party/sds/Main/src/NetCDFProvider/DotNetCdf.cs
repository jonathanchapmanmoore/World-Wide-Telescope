// Copyright © 2010 Microsoft Corporation, All Rights Reserved.
// This code released under the terms of the Microsoft Research License Agreement (MSR-LA, http://sds.codeplex.com/License)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Microsoft.Research.Science.Data.NetCDF4
{
	internal static partial class NetCDF
	{
		static System.Threading.Mutex m = new System.Threading.Mutex();

		public static int nc_open(string path, CreateMode mode, out int ncidp) { m.WaitOne(); var r = NetCDFInterop.nc_open(path, mode, out ncidp); m.ReleaseMutex(); return r; }

		public static int nc_create(string path, CreateMode mode, out int ncidp) { m.WaitOne(); var r = NetCDFInterop.nc_create(path, mode, out ncidp); m.ReleaseMutex(); return r; }

		public static int nc_close(int ncidp) { m.WaitOne(); var r = NetCDFInterop.nc_close(ncidp); m.ReleaseMutex(); return r; }

		public static int nc_sync(int ncid) { m.WaitOne(); var r = NetCDFInterop.nc_sync(ncid); m.ReleaseMutex(); return r; }

		public static int nc_enddef(int ncid) { m.WaitOne(); var r = NetCDFInterop.nc_enddef(ncid); m.ReleaseMutex(); return r; }

		public static int nc_redef(int ncid) { m.WaitOne(); var r = NetCDFInterop.nc_redef(ncid); m.ReleaseMutex(); return r; }

		public static int nc_inq(int ncid, out int ndims, out int nvars, out int ngatts, out int unlimdimid) { m.WaitOne(); var r = NetCDFInterop.nc_inq(ncid, out ndims, out nvars, out ngatts, out unlimdimid); m.ReleaseMutex(); return r; }

		public static int nc_def_var(int ncid, string name, NetCDF.NcType xtype, int ndims, int[] dimids, out int varidp) { m.WaitOne(); var r = NetCDFInterop.nc_def_var(ncid, name, xtype, ndims, dimids, out varidp); m.ReleaseMutex(); return r; }

		public static int nc_def_var_deflate(int ncid, int varid, int shuffle, int deflate, int deflate_level) { m.WaitOne(); var r = NetCDFInterop.nc_def_var_deflate(ncid, varid, shuffle, deflate, deflate_level); m.ReleaseMutex(); return r; }

		public static int nc_def_var_chunking(int ncid, int varid, int contiguous, IntPtr[] chunksizes) { m.WaitOne(); var r = NetCDFInterop.nc_def_var_chunking(ncid, varid, contiguous, chunksizes); m.ReleaseMutex(); return r; }

		public static int nc_inq_var(int ncid, int varid, StringBuilder name, out NetCDF.NcType type, out int ndims, int[] dimids, out int natts) { m.WaitOne(); var r = NetCDFInterop.nc_inq_var(ncid, varid, name, out type, out ndims, dimids, out natts); m.ReleaseMutex(); return r; }

		public static int nc_inq_varids(int ncid, out int nvars, int[] varids) { m.WaitOne(); var r = NetCDFInterop.nc_inq_varids(ncid, out nvars, varids); m.ReleaseMutex(); return r; }

		public static int nc_inq_vartype(int ncid, int varid, out NetCDF.NcType xtypep) { m.WaitOne(); var r = NetCDFInterop.nc_inq_vartype(ncid, varid, out xtypep); m.ReleaseMutex(); return r; }

		public static int nc_inq_varnatts(int ncid, int varid, out int nattsp) { m.WaitOne(); var r = NetCDFInterop.nc_inq_varnatts(ncid, varid, out nattsp); m.ReleaseMutex(); return r; }

		public static int nc_inq_varid(int ncid, string name, out int varidp) { m.WaitOne(); var r = NetCDFInterop.nc_inq_varid(ncid, name, out varidp); m.ReleaseMutex(); return r; }

		public static int nc_inq_ndims(int ncid, out int ndims) { m.WaitOne(); var r = NetCDFInterop.nc_inq_ndims(ncid, out ndims); m.ReleaseMutex(); return r; }

		public static int nc_inq_nvars(int ncid, out int nvars) { m.WaitOne(); var r = NetCDFInterop.nc_inq_nvars(ncid, out nvars); m.ReleaseMutex(); return r; }

		public static int nc_inq_varname(int ncid, int varid, StringBuilder name) { m.WaitOne(); var r = NetCDFInterop.nc_inq_varname(ncid, varid, name); m.ReleaseMutex(); return r; }

		public static int nc_inq_varndims(int ncid, int varid, out int ndims) { m.WaitOne(); var r = NetCDFInterop.nc_inq_varndims(ncid, varid, out ndims); m.ReleaseMutex(); return r; }

		public static int nc_inq_vardimid(int ncid, int varid, int[] dimids) { m.WaitOne(); var r = NetCDFInterop.nc_inq_vardimid(ncid, varid, dimids); m.ReleaseMutex(); return r; }

		public static int nc_inq_var_fill(int ncid, int varid, out int no_fill, out object fill_value) { m.WaitOne(); var r = NetCDFInterop.nc_inq_var_fill(ncid, varid, out no_fill, out fill_value); m.ReleaseMutex(); return r; }

		public static int nc_inq_natts(int ncid, out int ngatts) { m.WaitOne(); var r = NetCDFInterop.nc_inq_natts(ncid, out ngatts); m.ReleaseMutex(); return r; }

		public static int nc_inq_unlimdim(int ncid, out int unlimdimid) { m.WaitOne(); var r = NetCDFInterop.nc_inq_unlimdim(ncid, out unlimdimid); m.ReleaseMutex(); return r; }

		public static int nc_inq_format(int ncid, out int format) { m.WaitOne(); var r = NetCDFInterop.nc_inq_format(ncid, out format); m.ReleaseMutex(); return r; }

		public static int nc_inq_attname(int ncid, int varid, int attnum, StringBuilder name) { m.WaitOne(); var r = NetCDFInterop.nc_inq_attname(ncid, varid, attnum, name); m.ReleaseMutex(); return r; }

		public static int nc_inq_atttype(int ncid, int varid, string name, out NetCDF.NcType type) { m.WaitOne(); var r = NetCDFInterop.nc_inq_atttype(ncid, varid, name, out type); m.ReleaseMutex(); return r; }

		public static int nc_inq_att(int ncid, int varid, string name, out NetCDF.NcType type, out IntPtr length) { m.WaitOne(); var r = NetCDFInterop.nc_inq_att(ncid, varid, name, out type, out length); m.ReleaseMutex(); return r; }

		public static int nc_get_att_text(int ncid, int varid, string name, IntPtr value) { m.WaitOne(); var r = NetCDFInterop.nc_get_att_text(ncid, varid, name, value); m.ReleaseMutex(); return r; }

		public static int nc_get_att_schar(int ncid, int varid, string name, sbyte[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_att_schar(ncid, varid, name, data); m.ReleaseMutex(); return r; }

		public static int nc_get_att_uchar(int ncid, int varid, string name, byte[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_att_uchar(ncid, varid, name, data); m.ReleaseMutex(); return r; }

		public static int nc_get_att_short(int ncid, int varid, string name, short[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_att_short(ncid, varid, name, data); m.ReleaseMutex(); return r; }

		public static int nc_get_att_ushort(int ncid, int varid, string name, ushort[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_att_ushort(ncid, varid, name, data); m.ReleaseMutex(); return r; }

		public static int nc_get_att_int(int ncid, int varid, string name, int[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_att_int(ncid, varid, name, data); m.ReleaseMutex(); return r; }

		public static int nc_get_att_uint(int ncid, int varid, string name, uint[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_att_uint(ncid, varid, name, data); m.ReleaseMutex(); return r; }

		public static int nc_get_att_longlong(int ncid, int varid, string name, Int64[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_att_longlong(ncid, varid, name, data); m.ReleaseMutex(); return r; }

		public static int nc_get_att_ulonglong(int ncid, int varid, string name, UInt64[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_att_ulonglong(ncid, varid, name, data); m.ReleaseMutex(); return r; }

		public static int nc_get_att_float(int ncid, int varid, string name, float[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_att_float(ncid, varid, name, data); m.ReleaseMutex(); return r; }

		public static int nc_get_att_double(int ncid, int varid, string name, double[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_att_double(ncid, varid, name, data); m.ReleaseMutex(); return r; }

		public static int nc_del_att(int ncid, int varid, string name) { m.WaitOne(); var r = NetCDFInterop.nc_del_att(ncid, varid, name); m.ReleaseMutex(); return r; }

		public static int nc_put_att_text(int ncid, int varid, string name, IntPtr len, IntPtr tp) { m.WaitOne(); var r = NetCDFInterop.nc_put_att_text(ncid, varid, name, len, tp); m.ReleaseMutex(); return r; }

		public static int nc_put_att_string(int ncid, int varid, string name, IntPtr len, string[] op) { m.WaitOne(); var r = NetCDFInterop.nc_put_att_string(ncid, varid, name, len, op); m.ReleaseMutex(); return r; }

		public static int nc_put_att_double(int ncid, int varid, string name, NetCDF.NcType type, IntPtr len, double[] tp) { m.WaitOne(); var r = NetCDFInterop.nc_put_att_double(ncid, varid, name, type, len, tp); m.ReleaseMutex(); return r; }

		public static int nc_put_att_int(int ncid, int varid, string name, NetCDF.NcType type, IntPtr len, int[] tp) { m.WaitOne(); var r = NetCDFInterop.nc_put_att_int(ncid, varid, name, type, len, tp); m.ReleaseMutex(); return r; }

		public static int nc_put_att_short(int ncid, int varid, string name, NetCDF.NcType type, IntPtr len, short[] tp) { m.WaitOne(); var r = NetCDFInterop.nc_put_att_short(ncid, varid, name, type, len, tp); m.ReleaseMutex(); return r; }

		public static int nc_put_att_longlong(int ncid, int varid, string name, NetCDF.NcType type, IntPtr len, Int64[] tp) { m.WaitOne(); var r = NetCDFInterop.nc_put_att_longlong(ncid, varid, name, type, len, tp); m.ReleaseMutex(); return r; }

		public static int nc_put_att_ushort(int ncid, int varid, string name, NetCDF.NcType type, IntPtr len, UInt16[] tp) { m.WaitOne(); var r = NetCDFInterop.nc_put_att_ushort(ncid, varid, name, type, len, tp); m.ReleaseMutex(); return r; }

		public static int nc_put_att_uint(int ncid, int varid, string name, NetCDF.NcType type, IntPtr len, UInt32[] tp) { m.WaitOne(); var r = NetCDFInterop.nc_put_att_uint(ncid, varid, name, type, len, tp); m.ReleaseMutex(); return r; }

		public static int nc_put_att_float(int ncid, int varid, string name, NetCDF.NcType type, IntPtr len, float[] tp) { m.WaitOne(); var r = NetCDFInterop.nc_put_att_float(ncid, varid, name, type, len, tp); m.ReleaseMutex(); return r; }

		public static int nc_put_att_ulonglong(int ncid, int varid, string name, NetCDF.NcType type, IntPtr len, UInt64[] tp) { m.WaitOne(); var r = NetCDFInterop.nc_put_att_ulonglong(ncid, varid, name, type, len, tp); m.ReleaseMutex(); return r; }

		public static int nc_put_att_schar(int ncid, int varid, string name, NetCDF.NcType type, IntPtr len, sbyte[] tp) { m.WaitOne(); var r = NetCDFInterop.nc_put_att_schar(ncid, varid, name, type, len, tp); m.ReleaseMutex(); return r; }

		public static int nc_put_att_ubyte(int ncid, int varid, string name, NetCDF.NcType type, IntPtr len, byte[] tp) { m.WaitOne(); var r = NetCDFInterop.nc_put_att_ubyte(ncid, varid, name, type, len, tp); m.ReleaseMutex(); return r; }

		public static int nc_def_dim(int ncid, string name, IntPtr len, out int dimidp) { m.WaitOne(); var r = NetCDFInterop.nc_def_dim(ncid, name, len, out dimidp); m.ReleaseMutex(); return r; }

		public static int nc_inq_dim(int ncid, int dimid, StringBuilder name, out IntPtr length) { m.WaitOne(); var r = NetCDFInterop.nc_inq_dim(ncid, dimid, name, out length); m.ReleaseMutex(); return r; }

		public static int nc_inq_dimname(int ncid, int dimid, StringBuilder name) { m.WaitOne(); var r = NetCDFInterop.nc_inq_dimname(ncid, dimid, name); m.ReleaseMutex(); return r; }

		public static int nc_inq_dimid(int ncid, string name, out int dimid) { m.WaitOne(); var r = NetCDFInterop.nc_inq_dimid(ncid, name, out dimid); m.ReleaseMutex(); return r; }

		public static int nc_inq_dimlen(int ncid, int dimid, out IntPtr length) { m.WaitOne(); var r = NetCDFInterop.nc_inq_dimlen(ncid, dimid, out length); m.ReleaseMutex(); return r; }

		public static int nc_get_var_text(int ncid, int varid, byte[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_var_text(ncid, varid, data); m.ReleaseMutex(); return r; }

		public static int nc_get_att_string(int ncid, int varid, string name, IntPtr[] ip) { m.WaitOne(); var r = NetCDFInterop.nc_get_att_string(ncid, varid, name, ip); m.ReleaseMutex(); return r; }

		public static int nc_get_var_schar(int ncid, int varid, sbyte[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_var_schar(ncid, varid, data); m.ReleaseMutex(); return r; }

		public static int nc_get_var_short(int ncid, int varid, short[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_var_short(ncid, varid, data); m.ReleaseMutex(); return r; }

		public static int nc_get_var_int(int ncid, int varid, int[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_var_int(ncid, varid, data); m.ReleaseMutex(); return r; }

		public static int nc_get_var_long(int ncid, int varid, long[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_var_long(ncid, varid, data); m.ReleaseMutex(); return r; }

		public static int nc_get_var_float(int ncid, int varid, float[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_var_float(ncid, varid, data); m.ReleaseMutex(); return r; }

		public static int nc_get_var_double(int ncid, int varid, double[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_var_double(ncid, varid, data); m.ReleaseMutex(); return r; }

		public static int nc_put_vara_double(int ncid, int varid, IntPtr[] start, IntPtr[] count, double[] dp) { m.WaitOne(); var r = NetCDFInterop.nc_put_vara_double(ncid, varid, start, count, dp); m.ReleaseMutex(); return r; }

		public static int nc_put_vara_float(int ncid, int varid, IntPtr[] start, IntPtr[] count, float[] fp) { m.WaitOne(); var r = NetCDFInterop.nc_put_vara_float(ncid, varid, start, count, fp); m.ReleaseMutex(); return r; }

		public static int nc_put_vara_short(int ncid, int varid, IntPtr[] start, IntPtr[] count, short[] sp) { m.WaitOne(); var r = NetCDFInterop.nc_put_vara_short(ncid, varid, start, count, sp); m.ReleaseMutex(); return r; }

		public static int nc_put_vara_ushort(int ncid, int varid, IntPtr[] start, IntPtr[] count, ushort[] sp) { m.WaitOne(); var r = NetCDFInterop.nc_put_vara_ushort(ncid, varid, start, count, sp); m.ReleaseMutex(); return r; }

		public static int nc_put_vara_int(int ncid, int varid, IntPtr[] start, IntPtr[] count, int[] ip) { m.WaitOne(); var r = NetCDFInterop.nc_put_vara_int(ncid, varid, start, count, ip); m.ReleaseMutex(); return r; }

		public static int nc_put_vara_uint(int ncid, int varid, IntPtr[] start, IntPtr[] count, uint[] ip) { m.WaitOne(); var r = NetCDFInterop.nc_put_vara_uint(ncid, varid, start, count, ip); m.ReleaseMutex(); return r; }

		public static int nc_put_vara_longlong(int ncid, int varid, IntPtr[] start, IntPtr[] count, long[] lp) { m.WaitOne(); var r = NetCDFInterop.nc_put_vara_longlong(ncid, varid, start, count, lp); m.ReleaseMutex(); return r; }

		public static int nc_put_vara_ulonglong(int ncid, int varid, IntPtr[] start, IntPtr[] count, ulong[] lp) { m.WaitOne(); var r = NetCDFInterop.nc_put_vara_ulonglong(ncid, varid, start, count, lp); m.ReleaseMutex(); return r; }

		public static int nc_put_vara_ubyte(int ncid, int varid, IntPtr[] start, IntPtr[] count, byte[] bp) { m.WaitOne(); var r = NetCDFInterop.nc_put_vara_ubyte(ncid, varid, start, count, bp); m.ReleaseMutex(); return r; }

		public static int nc_put_vara_schar(int ncid, int varid, IntPtr[] start, IntPtr[] count, sbyte[] cp) { m.WaitOne(); var r = NetCDFInterop.nc_put_vara_schar(ncid, varid, start, count, cp); m.ReleaseMutex(); return r; }

		public static int nc_put_vara_string(int ncid, int varid, IntPtr[] start, IntPtr[] count, string[] sp) { m.WaitOne(); var r = NetCDFInterop.nc_put_vara_string(ncid, varid, start, count, sp); m.ReleaseMutex(); return r; }

		public static int nc_get_vara_text(int ncid, int varid, IntPtr[] start, IntPtr[] count, sbyte[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vara_text(ncid, varid, start, count, data); m.ReleaseMutex(); return r; }

		public static int nc_get_vara_schar(int ncid, int varid, IntPtr[] start, IntPtr[] count, sbyte[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vara_schar(ncid, varid, start, count, data); m.ReleaseMutex(); return r; }

		public static int nc_get_vara_short(int ncid, int varid, IntPtr[] start, IntPtr[] count, short[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vara_short(ncid, varid, start, count, data); m.ReleaseMutex(); return r; }

		public static int nc_get_vara_ushort(int ncid, int varid, IntPtr[] start, IntPtr[] count, ushort[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vara_ushort(ncid, varid, start, count, data); m.ReleaseMutex(); return r; }

		public static int nc_get_vara_ubyte(int ncid, int varid, IntPtr[] start, IntPtr[] count, byte[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vara_ubyte(ncid, varid, start, count, data); m.ReleaseMutex(); return r; }

		public static int nc_get_vara_longlong(int ncid, int varid, IntPtr[] start, IntPtr[] count, long[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vara_longlong(ncid, varid, start, count, data); m.ReleaseMutex(); return r; }

		public static int nc_get_vara_ulonglong(int ncid, int varid, IntPtr[] start, IntPtr[] count, ulong[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vara_ulonglong(ncid, varid, start, count, data); m.ReleaseMutex(); return r; }

		public static int nc_get_vara_int(int ncid, int varid, IntPtr[] start, IntPtr[] count, int[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vara_int(ncid, varid, start, count, data); m.ReleaseMutex(); return r; }

		public static int nc_get_vara_uint(int ncid, int varid, IntPtr[] start, IntPtr[] count, uint[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vara_uint(ncid, varid, start, count, data); m.ReleaseMutex(); return r; }

		public static int nc_get_vara_float(int ncid, int varid, IntPtr[] start, IntPtr[] count, float[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vara_float(ncid, varid, start, count, data); m.ReleaseMutex(); return r; }

		public static int nc_get_vara_double(int ncid, int varid, IntPtr[] start, IntPtr[] count, double[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vara_double(ncid, varid, start, count, data); m.ReleaseMutex(); return r; }

		public static int nc_get_vara_string(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vara_string(ncid, varid, start, count, data); m.ReleaseMutex(); return r; }

		public static int nc_get_vars_text(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, sbyte[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vars_text(ncid, varid, start, count, stride, data); m.ReleaseMutex(); return r; }

		public static int nc_get_vars_schar(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, sbyte[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vars_schar(ncid, varid, start, count, stride, data); m.ReleaseMutex(); return r; }

		public static int nc_get_vars_short(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, short[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vars_short(ncid, varid, start, count, stride, data); m.ReleaseMutex(); return r; }

		public static int nc_get_vars_ushort(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, ushort[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vars_ushort(ncid, varid, start, count, stride, data); m.ReleaseMutex(); return r; }

		public static int nc_get_vars_ubyte(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, byte[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vars_ubyte(ncid, varid, start, count, stride, data); m.ReleaseMutex(); return r; }

		public static int nc_get_vars_longlong(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, long[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vars_longlong(ncid, varid, start, count, stride, data); m.ReleaseMutex(); return r; }

		public static int nc_get_vars_ulonglong(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, ulong[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vars_ulonglong(ncid, varid, start, count, stride, data); m.ReleaseMutex(); return r; }

		public static int nc_get_vars_int(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, int[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vars_int(ncid, varid, start, count, stride, data); m.ReleaseMutex(); return r; }

		public static int nc_get_vars_uint(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, uint[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vars_uint(ncid, varid, start, count, stride, data); m.ReleaseMutex(); return r; }

		public static int nc_get_vars_float(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, float[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vars_float(ncid, varid, start, count, stride, data); m.ReleaseMutex(); return r; }

		public static int nc_get_vars_double(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, double[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vars_double(ncid, varid, start, count, stride, data); m.ReleaseMutex(); return r; }

		public static int nc_get_vars_string(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, IntPtr[] data) { m.WaitOne(); var r = NetCDFInterop.nc_get_vars_string(ncid, varid, start, count, stride, data); m.ReleaseMutex(); return r; }

		public static int nc_free_string(IntPtr len, IntPtr[] data) { m.WaitOne(); var r = NetCDFInterop.nc_free_string(len, data); m.ReleaseMutex(); return r; }

		public static string nc_strerror(int ncerror)
		{
			switch (ncerror)
			{
				case (0): return "No error";
				case (-1): return "Returned for all errors in the v2 API";
				case (-33): return "Not a netcdf id";
				case (-34): return "Too many netcdfs open";
				case (-35): return "netcdf file exists && NC_NOCLOBBER";
				case (-36): return "Invalid Argument";
				case (-37): return "Write to read only";
				case (-38): return "Operation not allowed in data mode";
				case (-39): return "Operation not allowed in define mode";
				case (-40): return "Index exceeds dimension bound. Consider cloning the file into NetCDF 4 format to enable data extdending (e.g. with sds copy command)";
				case (-41): return "NC_MAX_DIMS exceeded";
				case (-42): return "String match to name in use";
				case (-43): return "Attribute not found";
				case (-44): return "NC_MAX_ATTRS exceeded";
				case (-45): return "Not a netcdf data type. Some types are not supported by the classic NetCDF format. Consider cloning the file into NetCDF 4 format to enable use of all supported types (e.g. with sds copy command)";
				case (-46): return "Invalid dimension id or name";
				case (-47): return "NC_UNLIMITED in the wrong index";
				case (-48): return "NC_MAX_VARS exceeded";
				case (-49): return "Variable not found";
				case (-50): return "Action prohibited on NC_GLOBAL varid";
				case (-51): return "Not a netcdf file";
				case (-52): return "In Fortran, string too short";
				case (-53): return "NC_MAX_NAME exceeded";
				case (-54): return "NC_UNLIMITED size already in use";
				case (-55): return "nc_rec op when there are no record vars";
				case (-56): return "Attempt to convert between text & numbers";
				case (-57): return "Start+count exceeds dimension bound";
				case (-58): return "Illegal stride";
				case (-59): return "Attribute or variable name contains illegal characters";
				case (-60): return "Math result not representable";
				case (-61): return "Memory allocation (malloc) failure";
				case (-62): return "One or more variable sizes violate format constraints";
				case (-63): return "Invalid dimension size";
				case (-64): return "File likely truncated or possibly corrupted";
				case (-65): return "Unknown axis type.";
				// DAP errors
				case (-66): return "Generic DAP error";
				case (-67): return "Generic libcurl error";
				case (-68): return "Generic IO error";
				// netcdf-4 errors
				case (-100): return "NetCDF4 error";
				case (-101): return "Error at HDF5 layer.";
				case (-102): return "Can't read.";
				case (-103): return "Can't write.";
				case (-104): return "Can't create.";
				case (-105): return "Problem with file metadata.";
				case (-106): return "Problem with dimension metadata.";
				case (-107): return "Problem with attribute metadata.";
				case (-108): return "Problem with variable metadata.";
				case (-109): return "Not a compound type.";
				case (-110): return "Attribute already exists.";
				case (-111): return "Attempting netcdf-4 operation on netcdf-3 file.";
				case (-112): return "Attempting netcdf-4 operation on strict nc3 netcdf-4 file.";
				case (-113): return "Attempting netcdf-3 operation on netcdf-4 file.";
				case (-114): return "Parallel operation on file opened for non-parallel access.";
				case (-115): return "Error initializing for parallel access.";
				case (-116): return "Bad group ID.";
				case (-117): return "Bad type ID.";
				case (-118): return "Type has already been defined and may not be edited.";
				case (-119): return "Bad field ID.";
				case (-120): return "Bad class.";
				case (-121): return "Mapped access for atomic types only.";
				case (-122): return "Attempt to define fill value when data already exists.";
				case (-123): return "Attempt to define var properties, like deflate, after enddef.";
				case (-124): return "Probem with HDF5 dimscales.";
				case (-125): return "No group found.";
				case (-126): return "Can't specify both contiguous and chunking.";
				case (-127): return "Bad chunksize.";
				case (-128): return "NetCDF4 error";
				default: return "NetCDF error " + ncerror;
			}
		}
	}

	internal static partial class NetCDFInterop
	{
		[DllImport("netcdf4.dll")]
		public static extern int nc_open(string path, NetCDF.CreateMode mode, out int ncidp);
		[DllImport("netcdf4.dll")]
		public static extern int nc_create(string path, NetCDF.CreateMode mode, out int ncidp);
		[DllImport("netcdf4.dll")]
		public static extern int nc_close(int ncidp);
		[DllImport("netcdf4.dll")]
		public static extern int nc_sync(int ncid);
		[DllImport("netcdf4.dll")]
		public static extern int nc_enddef(int ncid);
		[DllImport("netcdf4.dll")]
		public static extern int nc_redef(int ncid);

		[DllImport("netcdf4.dll")]
		public static extern int nc_inq(int ncid, out int ndims, out int nvars, out int ngatts, out int unlimdimid);

		[DllImport("netcdf4.dll")]
		public static extern int nc_def_var(int ncid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
            string name,
			NetCDF.NcType xtype, int ndims, int[] dimids, out int varidp);

		[DllImport("netcdf4.dll")]
		public static extern int nc_def_var_deflate(int ncid, int varid, int shuffle, int deflate, int deflate_level);

		/// <summary>
		/// The function nc_def_var_chunking sets the chunking parameters for a variable in a netCDF-4 file.
		/// </summary>
		/// <remarks>
		/// This function must be called after the variable is defined, but before nc_enddef is called. Once the chunking parameters are set, they cannot be changed.
		/// Note that this does not work for scalar variables. Only non-scalar variables can have chunking.
		/// </remarks>
		/// <param name="ncid">NetCDF ID, from a previous call to nc_open or nc_create.</param>
		/// <param name="varid">Variable ID. </param>
		/// <param name="contiguous">If non-zero, then contiguous storage is used for this variable. Variables with one or more unlimited dimensions cannot use contiguous storage. If contiguous storage is turned on, the chunksizes parameter is ignored. </param>
		/// <param name="chunksizes">An array of chunk sizes. The array must have the one chunksize for each dimension in the variable. If the contiguous parameter is set, then the chunksizes parameter is ignored.</param>
		/// <returns></returns>
		[DllImport("netcdf4.dll")]
		public static extern int nc_def_var_chunking(int ncid, int varid, int contiguous, IntPtr[] chunksizes);
		[DllImport("netcdf4.dll")]
		// TODO: What's the size of name
		public static extern int nc_inq_var(int ncid, int varid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
            StringBuilder name,
			out NetCDF.NcType type, out int ndims, int[] dimids, out int natts);
		[DllImport("netcdf4.dll")]
		public static extern int nc_inq_varids(int ncid, out int nvars, int[] varids);
		[DllImport("netcdf4.dll")]
		public static extern int nc_inq_vartype(int ncid, int varid, out NetCDF.NcType xtypep);
		[DllImport("netcdf4.dll")]
		public static extern int nc_inq_varnatts(int ncid, int varid, out int nattsp);
		[DllImport("netcdf4.dll")]
		public static extern int nc_inq_varid(int ncid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
            string name,
			out int varidp);

		[DllImport("netcdf4.dll")]
		public static extern int nc_inq_ndims(int ncid, out int ndims);
		[DllImport("netcdf4.dll")]
		public static extern int nc_inq_nvars(int ncid, out int nvars);
		[DllImport("netcdf4.dll")]
		public static extern int nc_inq_varname(int ncid, int varid,
			StringBuilder name);
		[DllImport("netcdf4.dll")]
		public static extern int nc_inq_varndims(int ncid, int varid, out int ndims);
		[DllImport("netcdf4.dll")]
		public static extern int nc_inq_vardimid(int ncid, int varid, int[] dimids);
		[DllImport("netcdf4.dll")]
		public static extern int nc_inq_var_fill(int ncid, int varid, out int no_fill, out object fill_value);


		[DllImport("netcdf4.dll")]
		public static extern int nc_inq_natts(int ncid, out int ngatts);
		[DllImport("netcdf4.dll")]
		public static extern int nc_inq_unlimdim(int ncid, out int unlimdimid);
		[DllImport("netcdf4.dll")]
		public static extern int nc_inq_format(int ncid, out int format);

		[DllImport("netcdf4.dll")]
		// TODO: What's the size of attribute name
		public static extern int nc_inq_attname(int ncid, int varid, int attnum,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
            StringBuilder name);
		[DllImport("netcdf4.dll")]
		public static extern int nc_inq_atttype(int ncid, int varid, string name, out NetCDF.NcType type);
		[DllImport("netcdf4.dll")]
		public static extern int nc_inq_att(int ncid, int varid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
            string name, out NetCDF.NcType type, out IntPtr length);
		[DllImport("netcdf4.dll")]
		// TODO: Reimplement out string parameter
		public static extern int nc_get_att_text(int ncid, int varid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
            string name,
			IntPtr value);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_att_schar(int ncid, int varid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			string name, sbyte[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_att_uchar(int ncid, int varid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			string name, byte[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_att_short(int ncid, int varid, 
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			string name, short[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_att_ushort(int ncid, int varid, 
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			string name, ushort[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_att_int(int ncid, int varid, 
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			string name, int[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_att_uint(int ncid, int varid, 
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			string name, uint[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_att_longlong(int ncid, int varid, 
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			string name, Int64[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_att_ulonglong(int ncid, int varid, 
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			string name, UInt64[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_att_float(int ncid, int varid, 
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			string name, float[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_att_double(int ncid, int varid, 
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			string name, double[] data);

		[DllImport("netcdf4.dll")]
		public static extern int nc_del_att(int ncid, int varid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			string name);

		[DllImport("netcdf4.dll")]
		public static extern int nc_put_att_text(int ncid, int varid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
            string name,
			IntPtr len,
			//[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler), MarshalCookie="DontAppendZero")]
			//string tp);
			IntPtr tp);
		[DllImport("netcdf4.dll")]
		public static extern int nc_put_att_string(int ncid, int varid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
            string name,
			IntPtr len,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			String[] op);
		[DllImport("netcdf4.dll")]
		public static extern int nc_put_att_double(int ncid, int varid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			string name, NetCDF.NcType type, IntPtr len, double[] tp);
		[DllImport("netcdf4.dll")]
		public static extern int nc_put_att_int(int ncid, int varid, 
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			string name, NetCDF.NcType type, IntPtr len, int[] tp);
		[DllImport("netcdf4.dll")]
		public static extern int nc_put_att_short(int ncid, int varid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			string name, NetCDF.NcType type, IntPtr len, short[] tp);
		[DllImport("netcdf4.dll")]
		public static extern int nc_put_att_longlong(int ncid, int varid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			string name, NetCDF.NcType type, IntPtr len, Int64[] tp);
		[DllImport("netcdf4.dll")]
		public static extern int nc_put_att_ushort(int ncid, int varid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			string name, NetCDF.NcType type, IntPtr len, UInt16[] tp);
		[DllImport("netcdf4.dll")]
		public static extern int nc_put_att_uint(int ncid, int varid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			string name, NetCDF.NcType type, IntPtr len, UInt32[] tp);
		[DllImport("netcdf4.dll")]
		public static extern int nc_put_att_float(int ncid, int varid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			string name, NetCDF.NcType type, IntPtr len, float[] tp);
		[DllImport("netcdf4.dll")]
		public static extern int nc_put_att_ulonglong(int ncid, int varid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			string name, NetCDF.NcType type, IntPtr len, UInt64[] tp);
		[DllImport("netcdf4.dll")]
		public static extern int nc_put_att_schar(int ncid, int varid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			string name, NetCDF.NcType type, IntPtr len, sbyte[] tp);
		[DllImport("netcdf4.dll")]
		public static extern int nc_put_att_ubyte(int ncid, int varid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			string name, NetCDF.NcType type, IntPtr len, byte[] tp);

		[DllImport("netcdf4.dll")]
		public static extern int nc_def_dim(int ncid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			string name, IntPtr len, out int dimidp);
		[DllImport("netcdf4.dll")]
		public static extern int nc_inq_dim(int ncid, int dimid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]StringBuilder name, out IntPtr length);
		[DllImport("netcdf4.dll")]
		public static extern int nc_inq_dimname(int ncid, int dimid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]StringBuilder name);
		[DllImport("netcdf4.dll")]
		public static extern int nc_inq_dimid(int ncid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
			string name, out int dimid);
		[DllImport("netcdf4.dll")]
		public static extern int nc_inq_dimlen(int ncid, int dimid, out IntPtr length);


		[DllImport("netcdf4.dll")]
		public static extern int nc_get_var_text(int ncid, int varid, byte[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_att_string(int ncid, int varid,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]string name,
			IntPtr[] ip);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_var_schar(int ncid, int varid, sbyte[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_var_short(int ncid, int varid, short[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_var_int(int ncid, int varid, int[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_var_long(int ncid, int varid, long[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_var_float(int ncid, int varid, float[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_var_double(int ncid, int varid, double[] data);

		[DllImport("netcdf4.dll")]
		public static extern int nc_put_vara_double(int ncid, int varid, IntPtr[] start, IntPtr[] count, double[] dp);
		[DllImport("netcdf4.dll")]
		public static extern int nc_put_vara_float(int ncid, int varid, IntPtr[] start, IntPtr[] count, float[] fp);
		[DllImport("netcdf4.dll")]
		public static extern int nc_put_vara_short(int ncid, int varid, IntPtr[] start, IntPtr[] count, short[] sp);
		[DllImport("netcdf4.dll")]
		public static extern int nc_put_vara_ushort(int ncid, int varid, IntPtr[] start, IntPtr[] count, ushort[] sp);
		[DllImport("netcdf4.dll")]
		public static extern int nc_put_vara_int(int ncid, int varid, IntPtr[] start, IntPtr[] count, int[] ip);
		[DllImport("netcdf4.dll")]
		public static extern int nc_put_vara_uint(int ncid, int varid, IntPtr[] start, IntPtr[] count, uint[] ip);
		[DllImport("netcdf4.dll")]
		public static extern int nc_put_vara_longlong(int ncid, int varid, IntPtr[] start, IntPtr[] count, long[] lp);
		[DllImport("netcdf4.dll")]
		public static extern int nc_put_vara_ulonglong(int ncid, int varid, IntPtr[] start, IntPtr[] count, ulong[] lp);
		[DllImport("netcdf4.dll")]
		public static extern int nc_put_vara_ubyte(int ncid, int varid, IntPtr[] start, IntPtr[] count, byte[] bp);
		[DllImport("netcdf4.dll")]
		public static extern int nc_put_vara_schar(int ncid, int varid, IntPtr[] start, IntPtr[] count, sbyte[] cp);
		[DllImport("netcdf4.dll")]
		public static extern int nc_put_vara_string(int ncid, int varid, IntPtr[] start, IntPtr[] count,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]string[] sp);


		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vara_text(int ncid, int varid, IntPtr[] start, IntPtr[] count, sbyte[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vara_schar(int ncid, int varid, IntPtr[] start, IntPtr[] count, sbyte[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vara_short(int ncid, int varid, IntPtr[] start, IntPtr[] count, short[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vara_ushort(int ncid, int varid, IntPtr[] start, IntPtr[] count, ushort[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vara_ubyte(int ncid, int varid, IntPtr[] start, IntPtr[] count, byte[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vara_longlong(int ncid, int varid, IntPtr[] start, IntPtr[] count, long[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vara_ulonglong(int ncid, int varid, IntPtr[] start, IntPtr[] count, ulong[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vara_int(int ncid, int varid, IntPtr[] start, IntPtr[] count, int[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vara_uint(int ncid, int varid, IntPtr[] start, IntPtr[] count, uint[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vara_float(int ncid, int varid, IntPtr[] start, IntPtr[] count, float[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vara_double(int ncid, int varid, IntPtr[] start, IntPtr[] count, double[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vara_string(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] data);

		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vars_text(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, sbyte[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vars_schar(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, sbyte[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vars_short(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, short[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vars_ushort(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, ushort[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vars_ubyte(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, byte[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vars_longlong(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, long[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vars_ulonglong(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, ulong[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vars_int(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, int[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vars_uint(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, uint[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vars_float(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, float[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vars_double(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, double[] data);
		[DllImport("netcdf4.dll")]
		public static extern int nc_get_vars_string(int ncid, int varid, IntPtr[] start, IntPtr[] count, IntPtr[] stride, IntPtr[] data);


		[DllImport("netcdf4.dll")]
		public static extern int nc_free_string(IntPtr len, IntPtr[] data);
	}

}

