//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tivo.Has
{
    
    public struct HasApplicationConfiguration
    {
        private string _name;
        private System.Uri _endPoint;
        private string _assemblyQualifiedTypeName;
        public HasApplicationConfiguration(string name, System.Uri endPoint, string assemblyQualifiedTypeName)
        {
            _name = name;
            _endPoint = endPoint;
            _assemblyQualifiedTypeName = assemblyQualifiedTypeName;
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public System.Uri EndPoint
        {
            get
            {
                return _endPoint;
            }
            set
            {
                _endPoint = value;
            }
        }
        public string AssemblyQualifiedTypeName
        {
            get
            {
                return _assemblyQualifiedTypeName;
            }
            set
            {
                _assemblyQualifiedTypeName = value;
            }
        }
    }
}

