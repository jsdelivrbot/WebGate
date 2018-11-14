using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebGate.Bus
{
    #region Bus
    /// <summary>
    /// Шина окружения
    /// </summary>
    public class Bus : Atom, IBus
    {
        //public const string NameDefaultGroup = @"Common";
        //public const string NameServiceGroup = @"Service";

        private ProvAtomHolder atomHolder = new ProvAtomHolder();

        public Bus() : base("Common:Bus")
        {
            //atomHolder.RegisterProv<IBus>(this);
            Init();
        }

        #region init
        private void Init()
        {
            InitServices();
        }

        private void InitServices()
        {
            this.Register( new BusTypeDescriptor(typeof(IBus), "Service:Bus", "Application Bus"));  // Описатель интерфейса (Init Descriptor)
            this.Register( this, new List<Type>() { typeof(IBus) });                                // self made man  
        }
        #endregion init
        
        /// <summary>
        ///  Регистрирует объект (атом) со списком реализованных публичных интерфейсов 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="interfaces"></param>
        /// <returns></returns>
        public IResult Register(IAtom obj , List<Type> interfaces = null  )
        {
            return atomHolder.RegisterProv(obj, interfaces) ? Result.Ok() : Result.Error();
        }

        public string Info => atomHolder.Atoms.Aggregate("", (total, next) => total + "\n" + next.ToString());

        //return atomHolder.Atoms.Select<IAtom>.Aggregate("", (total, next) =>  total + "\n" + next.ToString());
        
    }

    #endregion Bus

    #region AtomBuilder 
    /// <summary>
    /// Entity
    /// </summary>
    public class AtomBuilder 
    {
        private Atom atom;

        public AtomBuilder()
        {
            atom = new Atom();
        }
        public AtomBuilder AddDescription(string desc)
        {
            atom.Description = desc;
            return this;
        }
        public AtomBuilder AddFromInstance(object obj)
        {
            var at = new Atom(obj.GetType().ToString(), obj.GetType().ToString());
            at.Description = string.IsNullOrEmpty(atom.Description) ? at.Description : atom.Description;
            return this;
        }
        public Atom Build()
        {
            return atom;
        }


    }

    #endregion AtomBuilder 

    #region Atom 
    /// <summary>
    /// Entity
    /// </summary>
    public class Atom : IAtom, IAtomDescriptor
    {
        public object Id { get; }
        public string Name { get; }
        public string Description { get; set; }

        IAtom IAtomDescriptor.Atom => this;

        public Atom(object id, string nаme, string description)
        {
            Id = id;
            Name = nаme;
            Description = description;
        }

        public Atom(string nаme, string description)
        {
            Id = Guid.NewGuid();
            Name = nаme;
            Description = description;
        }

        public Atom(string nаme)
        {
            Id = Guid.NewGuid();
            Name = nаme;
            Description = "";
        }

        public Atom()
        {
            Id = Guid.NewGuid(); ;
            Name = Id.ToString();
            Description = "";
        }

        public Atom(object snglobj)
        {
            Id = Guid.NewGuid(); ;
            Name = snglobj.GetType().ToString();
            Description = "";
        }

        public override string ToString()
        {
            return "id:" + Id.ToString() + ";  Name:" + Name + " [" + Description + "] ;";
        }
    }

    #endregion Atom 

    #region Atom<T> 
    /// <summary>
    /// Entity
    /// </summary>
    public class Atom<T> : Atom
    {
        public T Instance { get; } 

        public Atom(T val):base() 
        {
            Instance = val;
        }
        public Atom(T val, string nаme ) : base(nаme)
        {
            Instance = val;
        }

        public Atom(T val, string nаme, string description) : base(nаme, description)
        {
            Instance = val;
        }
    }
    #endregion Atom 

    #region BusTypeDescriptor
    /// <summary>
    /// Дескриптор типа 
    /// в узком смысле описатель интерфейса (реализуемого шиной)
    /// </summary>
    public class BusTypeDescriptor : Atom<Type>
    {
        public BusTypeDescriptor(Type iface, string name) : base(iface, name) { }
        public BusTypeDescriptor(Type iface, string name, string description) : base(iface, name, description) { }
    }
    #endregion BusTypeDescriptor

    #region AtomHolder
    public class AtomHolder
    {
        private Dictionary<object, IAtom> store = new Dictionary<object, IAtom>();
        private Dictionary<string, object> index = new Dictionary<string, object>();

        public IAtom this[string name] { get { return store[index[name]]; } }

        public IEnumerable<IAtom> Atoms => store.Values;

        public IAtom GetAtom(object id)
        {
            return store[id];
        }            

        

        public bool IsCanRegister(IAtom atom)
        {
            return (!store.ContainsKey(atom.Id) && !index.ContainsKey(atom.Name));
        }

        public bool IsContains(object id)
        {
            return store.ContainsKey(id);
        }

        public bool Register(IAtom atom)
        {
            return IsCanRegister(atom) && Reg(atom);
        }

        private bool Reg(IAtom atom)
        {
            bool ret = true;
            try
            {
                store.Add(atom.Id, atom);
                index.Add(atom.Name, atom.Id);
            }
            catch
            {
                ret = false;
            }
            return ret;
        }
    }
    #endregion AtomHolder

    #region ProvAtomHolder
    /// <summary>
    /// Провайдер 
    /// </summary>
    public class ProvAtomHolder : AtomHolder
    {
        private Dictionary<Type, List<object> > provider = new Dictionary<Type, List<object>>( );

        public bool IsCanRegisterAsProv<T>(IAtom atom)
        {
            return atom is T;
        }

        /// <summary>
        /// регистрит атом как провайдер интерфейса T
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="atom"></param>
        /// <returns></returns>
        public bool RegisterProv<I>(IAtom atom)
        {
            bool ret = false;
            if ( IsCanRegisterAsProv<I>(atom))
            {
                if (this.IsCanRegister(atom))
                {
                    this.Register(atom);
                    ret = true;
                }
                else
                {
                    if (this.IsContains(atom.Id))
                    {
                        atom = GetAtom(atom.Id);
                        ret = true;
                    }
                }

                if (ret)
                {
                    if (!provider.ContainsKey(typeof(I)))
                    {
                        provider.Add(typeof(I), new List<object>());
                    }
                    provider[typeof(I)].Add(atom.Id);
                }

            }
            return ret;
        }

        public bool RegisterProv(IAtom atom, List<Type> interfaces = null)
        {
            bool ret = Register(atom);
            if (interfaces != null)
            {
                foreach (var t in interfaces)
                {
                    var tp = this.GetType();
                    var method = tp.GetMethod("RegisterProv", new Type[] { typeof(IAtom) });
                    method = method.MakeGenericMethod(t);
                    method.Invoke(this, new object[] { atom });
                    ret = true;
                }
            }
            return ret;
        }
    }
    #endregion

    #region Result
    /// <summary>
    /// Entity
    /// </summary>
    public class Result : IResult
    {
        public bool    IsError { get; }
        public string  Message { get; }
        public IAtom   Owner   { get; }

        public Result(bool isError, string message, IAtom owner)
        {
            IsError = isError;
            Message = message;
            Owner   = owner;
        }

        public static Result Ok(IAtom owner = null)
        {
            return new Result(true, "", owner);
        }

        public static Result Error(string message= "" , IAtom owner = null)
        {
            return new Result(false, message, owner);
        }
    }

    public class Result<T> : Result, IResult<T>
    {
        T mydata;  
        T IResult<T>.Data { get { return mydata; } }

        public Result(T data, bool isError, string message, IAtom owner ):base(isError, message, owner)
        {
            mydata = data;
        }

        public static Result<T> Ok(T data, IAtom owner = null)
        {
            return new Result<T>(data, true, "", owner);
        }

        public static Result<T> Error(T data , string message = "", IAtom owner = null)
        {
            return new Result<T>(data, false, message, owner);
        }
    }


    #endregion Result 

}