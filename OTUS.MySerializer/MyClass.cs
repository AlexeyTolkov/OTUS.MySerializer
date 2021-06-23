namespace OTUS.MySerializer
{
    class MyClass
    { 
        public int i1, i2, i3, i4, i5;

        public int MyIntProperty { get; set; }
        public string MyStringProperty { get; set; }

        public static MyClass InitByDefault() => 
            new MyClass() 
            { 
                i1 = 1, 
                i2 = 2, 
                i3 = 3, 
                i4 = 4, 
                i5 = 5,
                MyIntProperty = 123,
                MyStringProperty = "myStringValue"
            }; 
    }
}