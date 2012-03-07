using System;
using System.Runtime.InteropServices;

namespace SimpleOT.Scripting
{
	public enum LuaGC : int {
		Stop = 0,
		Restart = 1,
		Collect = 2,
		Count = 3,
		CountB = 4,
		Step = 5,
		SetPause = 6,
		SetStepMul = 7
	}
	
	public enum LuaType : int {
		None = -1,
		Nil = 0,
		Boolean = 1,
		LightUserData = 2,
		Number = 3,
		String = 4,
		Table = 5,
		Function = 6,
		UserData = 7,
		Thread = 8
	}
	
	public enum LuaError : int {
		None = 0,
		Run = 2,
		Syntax = 3,
		MemoryAllocation = 4,
		Err = 5
	}
	
    public static class Lua
    {
        #region Constants

        /* mark for precompiled code (`<esc>Lua') */
        public const string SIGNATURE = "\033Lua";

        /* option for multiple returns in `lua_pcall' and `lua_call' */
        public const int MULTRET = (-1);

        /*
        ** pseudo-indices
        */
        public const int REGISTRYINDEX = (-10000);
        public const int ENVIRONINDEX = (-10001);
        public const int GLOBALSINDEX = (-10002);

        /* thread status; 0 is OK */
        public const int YIELD = 1;
        //public const int ERRRUN = 2;
        //public const int ERRSYNTAX = 3;
        //public const int ERRMEM = 4;
        //public const int ERRERR = 5;
		
        /*
         *  typedef int (*lua_CFunction) (state *L);
         */
        public delegate int LuaFunction(IntPtr state);

        /* minimum Lua stack available to a C function */
        public const int MINSTACK = 20;

        #endregion

        #region Extern Functions

		/// <summary>
		/// Destroys all objects in the given Lua state (calling the corresponding garbage-collection metamethods,
		/// if any) and frees all dynamic memory used by this state. On several platforms, you may not need to call this function,
		/// because all resources are naturally released when the host program ends. On the other hand, long-running programs,
		/// such as a daemon or a web server, might need to release states as soon as they are not needed, to avoid growing too large.
		/// </summary>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_close", CallingConvention=CallingConvention.Cdecl)]
        public static extern void Close(IntPtr state);

        /// <summary>
        /// Creates a new thread, pushes it on the stack, and returns a pointer to a lua_State that represents this new thread. 
        /// 
        /// The new state returned by this function shares with the original state all global objects (such as tables), 
        /// but has an independent execution stack. There is no explicit function to close or to destroy a thread. 
        /// 
        /// Threads are subject to garbage collection, like any Lua object.
        /// </summary>
        /// <returns>
        /// A pointer to a lua state that represents the new thread.
        /// </returns>
        /// <param name='state'>
        /// A pointer to a lua state.
        /// </param>
        [DllImport("lua52.dll", EntryPoint="lua_newthread", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr NewThread(IntPtr state);

		/// <summary>
		/// Sets a new panic function and returns the old one.
		/// 
		///	If an error happens outside any protected environment, Lua calls a panic function and then calls exit(EXIT_FAILURE),
		/// thus exiting the host application. Your panic function can avoid this exit by never returning (e.g., doing a long jump).
		/// 
		///	The panic function can access the error message at the top of the stack.
		/// </summary>
		/// <returns>
		/// The old panic function.
		/// </returns>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='panicFunction'>
		/// The new panic function.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_atpanic", CallingConvention=CallingConvention.Cdecl)]
        public static extern LuaFunction SetPanicFunction(IntPtr state, LuaFunction panicFunction);

        /// <summary>
        /// Returns the index of the top element in the stack. 
        /// 
        /// Because indices start at 1, this result is equal to the number of elements in the stack (and so 0 means an empty stack).
        /// </summary>
        /// <returns>
        /// Returns the index of the top element in the stack.
        /// </returns>
        /// <param name='state'>
        /// A pointer to a lua state.
        /// </param>
        [DllImport("lua52.dll", EntryPoint="lua_gettop", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetTop(IntPtr state);
		
		/// <summary>
		/// Accepts any acceptable index, or 0, and sets the stack top to this index. 
		/// 
		/// If the new top is larger than the old one, then the new elements are filled with nil. 
		/// If index is 0, then all stack elements are removed.
		/// </summary>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='index'>
		/// The new stack top index.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_settop", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetTop(IntPtr state, int index);
		
		/// <summary>
		/// Pushes a copy of the element at the given valid index onto the stack.
		/// </summary>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='index'>
		/// A stack index.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_pushvalue", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PushValue(IntPtr state, int index);
		
		/// <summary>
		/// Removes the element at the given valid index, shifting down the elements above this index to fill the gap. 
		/// Cannot be called with a pseudo-index, because a pseudo-index is not an actual stack position.
		/// </summary>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='index'>
		/// A stack index.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_remove", CallingConvention=CallingConvention.Cdecl)]
        public static extern void Remove(IntPtr state, int index);
		
		/// <summary>
		/// Moves the top element into the given valid index, shifting up the elements above this index to open space. 
		/// Cannot be called with a pseudo-index, because a pseudo-index is not an actual stack position.
		/// </summary>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='index'>
		/// A stack index.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_insert", CallingConvention=CallingConvention.Cdecl)]
        public static extern void Insert(IntPtr state, int index);
		
		/// <summary>
		/// Moves the top element into the given position (and pops it),
		/// without shifting any element (therefore replacing the value at the given position).
		/// </summary>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='index'>
		/// A stack index.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_replace", CallingConvention=CallingConvention.Cdecl)]
        public static extern void Replace(IntPtr state, int index);
		
		/// <summary>
		/// Ensures that there are at least extra free stack slots in the stack. 
		/// It returns false if it cannot grow the stack to that size. This function never shrinks the stack; 
		/// if the stack is already larger than the new size, it is left unchanged.
		/// </summary>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_checkstack", CallingConvention=CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CheckStack(IntPtr state);
        
        /// <summary>
        /// Returns <c>true</c> if the value at the given acceptable index is a number or a string convertible to a number, and <c>false</c> otherwise.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the value at the given acceptable index is a number or a string convertible to a number; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='state'>
        /// A pointer to a lua state.
        /// </param>
        /// <param name='index'>
        /// A stack index.
        /// </param>
        [DllImport("lua52.dll", EntryPoint="lua_isnumber", CallingConvention=CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsNumber(IntPtr state, int index);
		
        /// <summary>
        /// Returns <c>true</c> if the value at the given acceptable index is a string or a number (which is always convertible to a string), and <c>false</c> otherwise.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the value at the given acceptable index is a string or a number (which is always convertible to a string); otherwise, <c>false</c>.
        /// </returns>
        /// <param name='state'>
        /// A pointer to a lua state.
        /// </param>
        /// <param name='index'>
        /// A stack index.
        /// </param>
        [DllImport("lua52.dll", EntryPoint="lua_isstring", CallingConvention=CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsString(IntPtr state, int index);
		
        /// <summary>
        /// Returns <c>true</c> if the value at the given acceptable index is a C function, and <c>false</c> otherwise.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the value at the given acceptable index is a C function; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='state'>
        /// A pointer to a lua state.
        /// </param>
        /// <param name='index'>
        /// A stack index.
        /// </param>
        [DllImport("lua52.dll", EntryPoint="lua_iscfunction", CallingConvention=CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsCFunction(IntPtr state, int index);
		
        /// <summary>
        /// Returns <c>true</c> if the value at the given acceptable index is a userdata (either full or light), and <c>false</c> otherwise.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the value at the given acceptable index is a userdata (either full or light); otherwise, <c>false</c>.
        /// </returns>
        /// <param name='state'>
        /// A pointer to a lua state.
        /// </param>
        /// <param name='index'>
        /// A stack index.
        /// </param>
        [DllImport("lua52.dll", EntryPoint="lua_isuserdata", CallingConvention=CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsUserData(IntPtr state, int index);
		
		/// <summary>
		/// Returns the type of the value in the given acceptable index, or LUA_TNONE for a non-valid index 
		/// (that is, an index to an "empty" stack position). The types returned by lua_type are coded by the following constants defined 
		/// in lua.h: LUA_TNIL, LUA_TNUMBER, LUA_TBOOLEAN, LUA_TSTRING, LUA_TTABLE, LUA_TFUNCTION, LUA_TUSERDATA, LUA_TTHREAD, 
		/// and LUA_TLIGHTUSERDATA.
		/// </summary>
		/// <returns>
		/// The type of the value in the given acceptable index
		/// </returns>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='index'>
		/// A stack index.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_type", CallingConvention=CallingConvention.Cdecl)]
        public static extern LuaType GetType(IntPtr state, int index);
		
		/// <summary>
		/// Returns the name of the type encoded by the value tp, which must be one the values returned by lua_type.
		/// </summary>
		/// <returns>
		/// The name of the type
		/// </returns>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='type'>
		/// A valid lua type.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_typename", CallingConvention=CallingConvention.Cdecl)]
        public static extern string GetTypeName(IntPtr state, int type);
		
		/// <summary>
		/// Returns <c>true</c> if the two values in acceptable indices index1 and index2 are equal, following the semantics of the Lua == operator (that is, may call metamethods). Otherwise returns <c>false</c>. Also returns <c>false</c> if any of the indices is non valid.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the two values in acceptable indices index1 and index2 are equal; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='index1'>
		/// A stack index.
		/// </param>
		/// <param name='index2'>
		/// A stack index.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_equal", CallingConvention=CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsEqual(IntPtr state, int index1, int index2);
		
		/// <summary>
		/// Returns <c>true</c> if the two values in acceptable indices index1 and index2 are primitively equal (that is, without calling metamethods). Otherwise returns <c>false</c>. Also returns <c>false</c> if any of the indices is non valid.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the two values in acceptable indices index1 and index2 are primitively equal; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='index1'>
		/// A stack index.
		/// </param>
		/// <param name='index2'>
		/// A stack index.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_rawequal", CallingConvention=CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsRawEqual(IntPtr state, int index1, int index2);
		
		/// <summary>
		/// Returns <c>true</c> if the value at acceptable index index1 is smaller than the value at acceptable index index2, following the semantics of the Lua < operator (that is, may call metamethods). Otherwise returns <c>false</c>. Also returns <c>false</c> if any of the indices is non valid.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the value at acceptable index index1 is smaller than the value at acceptable index index2; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='index1'>
		/// A stack index.
		/// </param>
		/// <param name='index2'>
		/// A stack index.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_lessthan", CallingConvention=CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsLessThan(IntPtr state, int index1, int index2);
		
		/// <summary>
		/// Converts the Lua value at the given acceptable index to the C type lua_Number (see lua_Number).
		/// The Lua value must be a number or a string convertible to a number; otherwise, lua_tonumber returns 0.
		/// </summary>
		/// <returns>
		/// The converted number.
		/// </returns>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='index'>
		/// A stack index.
		/// </param>
        [DllImport("lua52.dll", EntryPoint = "lua_tonumberx", CallingConvention = CallingConvention.Cdecl)]
        public static extern double ToNumber(IntPtr state, int index, IntPtr isnum);
		
		/// <summary>
		/// Converts the Lua value at the given acceptable index to the signed integral type lua_Integer.
		/// The Lua value must be a number or a string convertible to a number; otherwise, lua_tointeger returns 0. 
		/// 
		/// If the number is not an integer, it is truncated in some non-specified way.
		/// </summary>
		/// <returns>
		/// The converted integer.
		/// </returns>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='index'>
		/// A stack index.
		/// </param>
        [DllImport("lua52.dll", EntryPoint = "lua_tointegerx", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ToInteger(IntPtr state, int index, IntPtr isnum);
		
        /// <summary>
        /// Converts the Lua value at the given acceptable index to a C boolean value (0 or 1). 
        /// Like all tests in Lua, lua_toboolean returns 1 for any Lua value different from false and nil; 
        /// otherwise it returns 0. It also returns 0 when called with a non-valid index. 
        /// (If you want to accept only actual boolean values, use lua_isboolean to test the value's type.)
        /// </summary>
        /// <returns>
        /// The converted value.
        /// </returns>
        /// <param name='state'>
        /// A pointer to a lua state.
        /// </param>
        /// <param name='index'>
        /// A stack index.
        /// </param>
        [DllImport("lua52.dll", EntryPoint="lua_toboolean", CallingConvention=CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ToBoolean(IntPtr state, int index);
		
		/// <summary>
		/// Converts the Lua value at the given acceptable index to a C string. 
		/// If len is not NULL, it also sets *len with the string length. The Lua value must be a string or a number; 
		/// otherwise, the function returns NULL. If the value is a number, then lua_tolstring also changes the actual 
		/// value in the stack to a string. (This change confuses lua_next when lua_tolstring is applied to keys during a table traversal.)
		/// 
		/// lua_tolstring returns a fully aligned pointer to a string inside the Lua state. 
		/// This string always has a zero ('\0') after its last character (as in C), but can contain other zeros in its body. 
		/// Because Lua has garbage collection, there is no guarantee that the pointer returned by lua_tolstring will be valid 
		/// after the corresponding value is removed from the stack.
		/// </summary>
		/// <returns>
		/// The new string.
		/// </returns>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='index'>
		/// A stack index.
		/// </param>
		/// <param name='len'>
		/// The new string length.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_tolstring", CallingConvention=CallingConvention.Cdecl)]
        public static extern string ToString(IntPtr state, int index, UIntPtr len);
		
		/// <summary>
		/// Returns the "length" of the value at the given acceptable index: for strings, 
		/// this is the string length; for tables, this is the result of the length operator ('#'); 
		/// for userdata, this is the size of the block of memory allocated for the userdata; for other values, it is 0.
		/// </summary>
		/// <returns>
		/// The object length.
		/// </returns>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='index'>
		/// A stack index.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_objlen", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetObjectLength(IntPtr state, int index);
		
        /// <summary>
        /// Converts a value at the given acceptable index to a <c>LuaFunction</c> function. That value must be a <c>LuaFunction</c> function; otherwise, returns <c>null</c>.
        /// </summary>
        /// <param name='state'>
        /// A pointer to a lua state.
        /// </param>
        /// <param name='index'>
        /// A stack index.
        /// </param>
        [DllImport("lua52.dll", EntryPoint="lua_tocfunction", CallingConvention=CallingConvention.Cdecl)]
        public static extern LuaFunction ToCFunction(IntPtr state, int index);
		
        /// <summary>
        /// If the value at the given acceptable index is a full userdata, returns its block address. 
        /// If the value is a light userdata, returns its pointer. Otherwise, returns <c>null</c>.
        /// </summary>
        /// <returns>
        /// A pointer to the user data.
        /// </returns>
        /// <param name='state'>
        /// A pointer to a lua state.
        /// </param>
        /// <param name='index'>
        /// A stack index.
        /// </param>
        [DllImport("lua52.dll", EntryPoint="lua_touserdata", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr ToUserData(IntPtr state, int index);
		
        /// <summary>
        /// Converts the value at the given acceptable index to a Lua thread (represented as lua_State*). 
        /// This value must be a thread; otherwise, the function returns NULL.
        /// </summary>
        /// <returns>
        /// The thread.
        /// </returns>
        /// <param name='state'>
        /// A pointer to a lua state.
        /// </param>
        /// <param name='index'>
        /// A stack index.
        /// </param>
        [DllImport("lua52.dll", EntryPoint="lua_tothread", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr ToThread(IntPtr state, int index);
		
        /// <summary>
        /// Converts the value at the given acceptable index to a generic C pointer (void*).
        /// The value can be a userdata, a table, a thread, or a function; otherwise, lua_topointer returns NULL. 
        /// Different objects will give different pointers. There is no way to convert the pointer back to its original value.
        /// </summary>
        /// <returns>
        /// The pointer.
        /// </returns>
        /// <param name='state'>
        /// A pointer to a lua state.
        /// </param>
        /// <param name='index'>
        /// A stack index.
        /// </param>
        [DllImport("lua52.dll", EntryPoint="lua_topointer", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr ToPointer(IntPtr state, int index);

		/// <summary>
		/// Pushes a nil value onto the stack.
		/// </summary>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_pushnil", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PushNil(IntPtr state);
		
		/// <summary>
		/// Pushes a number with value n onto the stack.
		/// </summary>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='n'>
		/// The value to be pushed.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_pushnumber", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PushNumber(IntPtr state, double n);

		/// <summary>
		/// Pushes a number with value n onto the stack.
		/// </summary>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='n'>
		/// The value to be pushed.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_pushinteger", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PushInteger(IntPtr state, int n);
		
		/// <summary>
		/// Pushes the zero-terminated string pointed to by s onto the stack. 
		/// Lua makes (or reuses) an internal copy of the given string, so the memory at s can be freed or 
		/// reused immediately after the function returns. The string cannot contain embedded zeros; it is 
		/// assumed to end at the first zero.
		/// </summary>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='value'>
		/// The value to be pushed.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_pushstring", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PushString(IntPtr state, string value);
		
		/// <summary>
		/// Pushes a new C closure onto the stack. 
		/// 
		/// When a C function is created, it is possible to associate some values with it, thus creating a C closure;
		/// these values are then accessible to the function whenever it is called. To associate values with a C function, 
		/// first these values should be pushed onto the stack (when there are multiple values, the first value is pushed first). 
		/// Then lua_pushcclosure is called to create and push the C function onto the stack, with the argument n telling how many 
		/// values should be associated with the function. lua_pushcclosure also pops these values from the stack.
		/// 
		/// The maximum value for n is 255.
		/// </summary>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='func'>
		/// The function to be pushed.
		/// </param>
		/// <param name='n'>
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_pushcclosure", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PushCClosure(IntPtr state, [MarshalAs(UnmanagedType.FunctionPtr)] LuaFunction func, int n);

		/// <summary>
		/// Pushes a boolean value with value b onto the stack.
		/// </summary>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='value'>
		/// The value to be pushed.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_pushboolean", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PushBoolean(IntPtr state, [MarshalAs(UnmanagedType.Bool)] bool b);

		/// <summary>
		/// Pushes onto the stack the value t[k], where t is the value at the given valid index and k
		/// is the value at the top of the stack.
		/// 
		/// This function pops the key from the stack (putting the resulting value in its place). 
		/// As in Lua, this function may trigger a metamethod for the "index" event.
		/// </summary>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='index'>
		/// A stack index.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_gettable", CallingConvention=CallingConvention.Cdecl)]
        public static extern void GetTable(IntPtr state, int index);
		
		/// <summary>
		/// Pushes onto the stack the value t[k], where t is the value at the given valid index. 
		/// As in Lua, this function may trigger a metamethod for the "index" event.
		/// </summary>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='index'>
		/// A stack index.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_getfield", CallingConvention=CallingConvention.Cdecl)]
        public static extern void GetField(IntPtr state, int index, string s);
		
		/// <summary>
		/// Similar to lua_gettable, but does a raw access (i.e., without metamethods).
		/// </summary>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='index'>
		/// A stack index.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_rawget", CallingConvention=CallingConvention.Cdecl)]
        public static extern void GetRaw(IntPtr state, int index);
		
		/// <summary>
		/// Pushes onto the stack the value t[n], where t is the value at the given valid index. The access is raw; that is, it does not invoke metamethods.
		/// </summary>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='index'>
		/// A stack index.
		/// </param>
		/// <param name='n'>
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_rawgeti", CallingConvention=CallingConvention.Cdecl)]
        public static extern void GetRaw(IntPtr state, int index, int n);
		
		/// <summary>
		/// Creates a new empty table and pushes it onto the stack. The new table has space pre-allocated for
		/// narr array elements and nrec non-array elements. This pre-allocation is useful when you know exactly 
		/// how many elements the table will have. Otherwise you can use the function lua_newtable.
		/// </summary>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='narr'>
		/// The number of array elements.
		/// </param>
		/// <param name='nrec'>
		/// The number of non-array elements.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_createtable", CallingConvention=CallingConvention.Cdecl)]
        public static extern void CreateTable(IntPtr state, int narr, int nrec);
		
		/// <summary>
		/// Pushes onto the stack the metatable of the value at the given acceptable index. 
		/// If the index is not valid, or if the value does not have a metatable, the function returns 0 
		/// and pushes nothing on the stack.
		/// </summary>
		/// <returns>
		/// The meta table.
		/// </returns>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='index'>
		/// A stack index.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_getmetatable", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetMetaTable(IntPtr state, int index);
		
        [DllImport("lua52.dll", EntryPoint="lua_getfenv", CallingConvention=CallingConvention.Cdecl)]
        public static extern void GetEnvironmentTable(IntPtr state, int index);

        [DllImport("lua52.dll", EntryPoint="lua_settable", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetTable(IntPtr state, int index);
		
        [DllImport("lua52.dll", EntryPoint="lua_setfield", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetField(IntPtr state, int index, string s);

        [DllImport("lua52.dll", EntryPoint="lua_rawset", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetRaw(IntPtr state, int index);
		
        [DllImport("lua52.dll", EntryPoint="lua_rawseti", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetRaw(IntPtr state, int index, int n);
		
        [DllImport("lua52.dll", EntryPoint="lua_setmetatable", CallingConvention=CallingConvention.Cdecl)]
        public static extern int SetMetaTable(IntPtr state, int index);
		
        [DllImport("lua52.dll", EntryPoint="lua_setfenv", CallingConvention=CallingConvention.Cdecl)]
        public static extern int SetEnvironmentTable(IntPtr state, int index);

		/// <summary>
		/// Calls a function.
		///
		/// To call a function you must use the following protocol: first, the function to be called is pushed onto the stack; 
		/// then, the arguments to the function are pushed in direct order; that is, the first argument is pushed first. 
		/// Finally you call lua_call; nargs is the number of arguments that you pushed onto the stack. All arguments and 
		/// the function value are popped from the stack when the function is called. The function results are pushed onto 
		/// the stack when the function returns. The number of results is adjusted to nresults, unless nresults is LUA_MULTRET. 
		/// In this case, all results from the function are pushed. Lua takes care that the returned values fit into the stack space. 
		/// The function results are pushed onto the stack in direct order (the first result is pushed first), so that after the 
		/// call the last result is on the top of the stack.
		/// 
		/// Any error inside the called function is propagated upwards (with a longjmp).
		/// 
		/// The following example shows how the host program can do the equivalent to this Lua code:
		/// 
		///      a = f("how", t.x, 14)
		/// Here it is in C:
		/// 
		///      lua_getfield(L, LUA_GLOBALSINDEX, "f"); /* function to be called */
		///      lua_pushstring(L, "how");                        /* 1st argument */
		///      lua_getfield(L, LUA_GLOBALSINDEX, "t");   /* table to be indexed */
		///      lua_getfield(L, -1, "x");        /* push result of t.x (2nd arg) */
		///      lua_remove(L, -2);                  /* remove 't' from the stack */
		///      lua_pushinteger(L, 14);                          /* 3rd argument */
		///      lua_call(L, 3, 1);     /* call 'f' with 3 arguments and 1 result */
		///      lua_setfield(L, LUA_GLOBALSINDEX, "a");        /* set global 'a' */
		/// 
		/// Note that the code above is "balanced": at its end, the stack is back to its original configuration. 
		/// This is considered good programming practice.
		/// </summary>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='nargs'>
		/// The number of arguments
		/// </param>
		/// <param name='nresults'>
		/// The number of results
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_call", CallingConvention=CallingConvention.Cdecl)]
        public static extern void Call(IntPtr state, int nargs, int nresults);
		
		/// <summary>
		/// Calls a function in protected mode.
		/// 
		/// Both nargs and nresults have the same meaning as in lua_call. If there are no errors during the call, 
		/// lua_pcall behaves exactly like lua_call. However, if there is any error, lua_pcall catches it, pushes 
		/// a single value on the stack (the error message), and returns an error code. Like lua_call, lua_pcall 
		/// always removes the function and its arguments from the stack.
		/// 
		/// If errfunc is 0, then the error message returned on the stack is exactly the original error message. 
		/// Otherwise, errfunc is the stack index of an error handler function. (In the current implementation, 
		/// this index cannot be a pseudo-index.) In case of runtime errors, this function will be called with 
		/// the error message and its return value will be the message returned on the stack by lua_pcall.
		/// 
		/// Typically, the error handler function is used to add more debug information to the error message, 
		/// such as a stack traceback. Such information cannot be gathered after the return of lua_pcall, since 
		/// by then the stack has unwound.
		/// 
		/// The lua_pcall function returns 0 in case of success or one of the following error codes (defined in lua.h):
		/// 
		/// LUA_ERRRUN: a runtime error.
		/// LUA_ERRMEM: memory allocation error. For such errors, Lua does not call the error handler function.
		/// LUA_ERRERR: error while running the error handler function.
		/// 
		/// </summary>
		/// <returns>
		/// The call.
		/// </returns>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='nargs'>
		/// The number of arguments
		/// </param>
		/// <param name='nresults'>
		/// The number of results
		/// </param>
		/// <param name='errfunc'>
		/// Errfunc.
		/// </param>
        [DllImport("lua52.dll", EntryPoint = "lua_pcallk", CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaError ProtectedCall(IntPtr state, int nargs, int nresults, int errfunc, int ctx, LuaFunction k);
		
        [DllImport("lua52.dll", EntryPoint="lua_gc", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GC(IntPtr state, int what, int data);

        [DllImport("lua52.dll", EntryPoint="lua_error", CallingConvention=CallingConvention.Cdecl)]
        public static extern int Error(IntPtr state);
		
        [DllImport("lua52.dll", EntryPoint="lua_next", CallingConvention=CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool Next(IntPtr state, int index);

		/// <summary>
		/// Concatenates the n values at the top of the stack, pops them, and leaves the result at the top. 
		/// If n is 1, the result is the single value on the stack (that is, the function does nothing); 
		/// if n is 0, the result is the empty string. Concatenation is performed following the usual semantics of Lua
		/// </summary>
		/// <param name='state'>
		/// State.
		/// </param>
		/// <param name='n'>
		/// The number of values.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="lua_concat", CallingConvention=CallingConvention.Cdecl)]
        public static extern void Concat(IntPtr state, int n);
		
		/// <summary>
		/// Loads a string as a Lua chunk. This function uses lua_load to load the chunk in the zero-terminated string s.
		/// 
		/// This function returns the same results as lua_load.
		/// 
		/// Also as lua_load, this function only loads the chunk; it does not run it.
		/// </summary>
		/// <returns>
		/// The string.
		/// </returns>
		/// <param name='state'>
		/// A pointer to a lua state.
		/// </param>
		/// <param name='s'>
		/// The string to be loaded.
		/// </param>
        [DllImport("lua52.dll", EntryPoint="luaL_loadstring", CallingConvention=CallingConvention.Cdecl)]
        public static extern LuaError LoadString(IntPtr state, string s);

        [DllImport("lua52.dll", EntryPoint="luaL_newstate", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr NewState();

        [DllImport("lua52.dll", EntryPoint="luaopen_base", CallingConvention=CallingConvention.Cdecl)]
        public static extern int OpenBase(IntPtr state);

        [DllImport("lua52.dll", EntryPoint="luaopen_debug", CallingConvention=CallingConvention.Cdecl)]
        public static extern int OpenDebug(IntPtr state);

        [DllImport("lua52.dll", EntryPoint="luaopen_io", CallingConvention=CallingConvention.Cdecl)]
        public static extern int OpenIO(IntPtr state);

        [DllImport("lua52.dll", EntryPoint="luaopen_math", CallingConvention=CallingConvention.Cdecl)]
        public static extern int OpenMath(IntPtr state);

        [DllImport("lua52.dll", EntryPoint="luaopen_os", CallingConvention=CallingConvention.Cdecl)]
        public static extern int OpenOS(IntPtr state);

        [DllImport("lua52.dll", EntryPoint="luaopen_package", CallingConvention=CallingConvention.Cdecl)]
        public static extern int OpenPackage(IntPtr state);

        [DllImport("lua52.dll", EntryPoint="luaopen_string", CallingConvention=CallingConvention.Cdecl)]
        public static extern int OpenString(IntPtr state);

        [DllImport("lua52.dll", EntryPoint="luaopen_table", CallingConvention=CallingConvention.Cdecl)]
        public static extern int OpenTable(IntPtr state);

        [DllImport("lua52.dll", EntryPoint="luaL_openlibs", CallingConvention=CallingConvention.Cdecl)]
        public static extern void OpenLibs(IntPtr state);

        [DllImport("lua52.dll", EntryPoint = "luaL_loadfilex", CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaError LoadFile(IntPtr state, string fileName, string mode);

        [DllImport("lua52.dll", EntryPoint = "lua_getglobal", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetGlobal(IntPtr state, string s);

        #endregion

        #region Macros

        public static double ToNumber(IntPtr state, int index)
        {
            return ToNumber(state, index, IntPtr.Zero);
        }

        public static int ToInteger(IntPtr state, int index)
        {
            return ToInteger(state, index, IntPtr.Zero);
        }

        public static void Pop(IntPtr state, int amount)
        {
            SetTop(state, -(amount) - 1);
        }

        public static void NewTable(IntPtr state)
        {
            CreateTable(state, 0, 0);
        }

        public static void Register(IntPtr state, string n, LuaFunction func)
        {
            PushCFunction(state, func);
            SetGlobal(state, n);
        }

        public static void PushCFunction(IntPtr state, LuaFunction func)
        {
            PushCClosure(state, func, 0);
        }

        public static void GetStringLength(IntPtr state, int i)
        {
            GetObjectLength(state, i);
        }

        public static bool IsFunction(IntPtr state, int n)
        {
            return GetType(state, n) == LuaType.Function;
        }
		
        public static bool IsTable(IntPtr state, int n)
        {
            return GetType(state, n) == LuaType.Table;
        }
		
        public static bool IsNil(IntPtr state, int n)
        {
            return GetType(state, n) == LuaType.Nil;
        }
		
        public static bool IsBoolean(IntPtr state, int n)
        {
            return GetType(state, n) == LuaType.Boolean;
        }

        public static bool IsNone(IntPtr state, int n)
        {
            return GetType(state, n) == LuaType.None;
        }

        public static bool IsNoneOrNil(IntPtr state, int n)
        {
            return GetType(state, n) <= 0;
        }

        public static void SetGlobal(IntPtr state, string s)
        {
            SetField(state, GLOBALSINDEX, s);
        }

        public static string GetGlobalString(IntPtr state, string s)
        {
            GetGlobal(state, s);

            if (!IsString(state, -1))
            {
                Pop(state, 1);
                return null;
            }

            var ret = ToString(state, -1);
            Pop(state, 1);

            return ret;
        }

        public static long GetGlobalLong(IntPtr state, string s)
        {
            GetGlobal(state, s);

            if (!IsNumber(state, -1))
            {
                Pop(state, 1);
                return 0;
            }

            var ret = (long)ToNumber(state, -1);
            Pop(state, 1);

            return ret;
        }

        public static int GetGlobalInteger(IntPtr state, string s)
        {
            GetGlobal(state, s);

            if (!IsNumber(state, -1))
            {
                Pop(state, 1);
                return 0;
            }

            var ret = ToInteger(state, -1);
            Pop(state, 1);

            return ret;
        }

        public static double GetGlobalDouble(IntPtr state, string s)
        {
            GetGlobal(state, s);

            if (!IsNumber(state, -1))
            {
                Pop(state, 1);
                return 0D;
            }

            var ret = ToNumber(state, -1);
            Pop(state, 1);

            return ret;
        }

        public static string ToString(IntPtr state, int i)
        {
            return ToString(state, i, UIntPtr.Zero);
        }

        public static IntPtr Open()
        {
            return NewState();
        }

        public static LuaError DoString(IntPtr state, string s)
        {
            var ret = LoadString(state, s);

            if (ret != LuaError.None)
                return ret;

            return ProtectedCall(state, 0, MULTRET, 0, 0, null);
        }

        public static LuaError DoFile(IntPtr state, string fileName)
        {
            var ret = LoadFile(state, fileName, null);
            
            if (ret != LuaError.None)
                return ret;

            return ProtectedCall(state, 0, MULTRET, 0, 0, null);
        }

        public static void MoveValue(IntPtr from, IntPtr to)
        {
            switch (GetType(from, -1))
            {
                case LuaType.Nil:
                    PushNil(to);
                    break;
                case LuaType.Boolean:
                    PushBoolean(to, ToBoolean(from, -1));
                    break;
                case LuaType.Number:
                    PushNumber(to, ToNumber(from, -1));
                    break;
                case LuaType.String:
                    {
                        //var len = UIntPtr.Zero;
                        //var str = Lua.lua_tolstring(from, -1, len);
                        PushString(to, ToString(from, -1));
                    }
                    break;
                case LuaType.Table:
                    NewTable(to);

                    PushNil(from); // First key
                    while (Next(from, -2))
                    {
                        // Move value to the other state
                        MoveValue(from, to);
                        // Value is popped, key is left

                        // Move key to the other state
                        PushValue(from, -1); // Make a copy of the key to use for the next iteration
                        MoveValue(from, to);
                        // Key is in other state.
                        // We still have the key in the 'from' state ontop of the stack

                        Insert(to, -2); // Move key above value
                        SetTable(to, -3); // Set the key
                    }
                    break;
            }
            // Pop the value we just read
            Pop(from, 1);
        }

        public static string PopString(IntPtr state)
        {
            Pop(state, 1);
            var value = string.Empty;

            if (IsString(state, 0))
                value = ToString(state, 0);

            return value;
        }

        #endregion
    }
}
