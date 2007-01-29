using System;

namespace StructureMap.Testing.Widget4
{
    public interface IStrategy
    {
        void DoSomething();
    }


    public class Strategy : IStrategy
    {
        private string _name;
        private int _count;
        private double _rating;
        private long _quantity;
        private bool _isCalculated;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        public double Rating
        {
            get { return _rating; }
            set { _rating = value; }
        }

        public long Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }

        public bool IsCalculated
        {
            get { return _isCalculated; }
            set { _isCalculated = value; }
        }

        public Strategy(string name, int count, double rating, long quantity, bool isCalculated)
        {
            _name = name;
            _count = count;
            _rating = rating;
            _quantity = quantity;
            _isCalculated = isCalculated;
        }

        #region IStrategy Members

        public void DoSomething()
        {
            // TODO:  Add Strategy.DoSomething implementation
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj is Strategy)
            {
                Strategy peer = (Strategy) obj;

                bool returnValue = Name.Equals(peer.Name);
                returnValue = returnValue && Count.Equals(peer.Count);
                returnValue = returnValue && Rating.Equals(peer.Rating);
                returnValue = returnValue && Quantity.Equals(peer.Quantity);
                returnValue = returnValue && IsCalculated.Equals(peer.IsCalculated);

                return returnValue;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public enum StrategyType
    {
        LongTerm,
        ShortTerm,
        OverTheHorizon
    }


    public class ComplexStrategy : IStrategy
    {
        private readonly StrategyType _strategyType;
        private readonly IStrategy _defaultStrategy;
        private readonly long _quantity;
        private readonly string _name;
        private readonly IStrategy[] _innerStrategies;

        public ComplexStrategy(IStrategy[] innerStrategies, string name, long quantity,
                               IStrategy defaultStrategy, StrategyType strategyType)
        {
            _innerStrategies = innerStrategies;
            _name = name;
            _quantity = quantity;
            _defaultStrategy = defaultStrategy;
            _strategyType = strategyType;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IStrategy Members

        public void DoSomething()
        {
            // TODO:  Add ComplexStrategy.DoSomething implementation
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj is ComplexStrategy)
            {
                ComplexStrategy peer = (ComplexStrategy) obj;
                if (_innerStrategies.Length != peer._innerStrategies.Length)
                {
                    return false;
                }

                bool returnValue = _strategyType.Equals(peer._strategyType);
                returnValue = returnValue && _defaultStrategy.Equals(peer._defaultStrategy);
                returnValue = returnValue && _quantity.Equals(peer._quantity);
                returnValue = returnValue && _name.Equals(peer._name);

                for (int i = 0; i < _innerStrategies.Length; i++)
                {
                    returnValue = returnValue && _innerStrategies[i].Equals(peer._innerStrategies[i]);
                }

                return returnValue;
            }
            else
            {
                return false;
            }
        }
    }


    public class RandomStrategy : IStrategy
    {
        public double Seed
        {
            get { return _seed; }
            set { _seed = value; }
        }

        public RandomStrategy(double seed)
        {
            _seed = seed;
        }


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private double _seed;

        #region IStrategy Members

        public void DoSomething()
        {
            // TODO:  Add RandomStrategy.DoSomething implementation
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj is RandomStrategy)
            {
                RandomStrategy peer = (RandomStrategy) obj;
                return (Seed == peer.Seed);
            }
            else
            {
                return false;
            }
        }
    }


    public class ColorStrategy : IStrategy
    {
        private string _color;

        public string Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public ColorStrategy(string color)
        {
            _color = color;
        }

        #region IStrategy Members

        public void DoSomething()
        {
            // TODO:  Add ColorStrategy.DoSomething implementation
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj is ColorStrategy)
            {
                ColorStrategy peer = (ColorStrategy) obj;
                return (Color == peer.Color);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }


    public class StrategyDecorator : IStrategy
    {
        public IStrategy InnerStrategy
        {
            get { return _innerStrategy; }
            set { _innerStrategy = value; }
        }

        public StrategyDecorator(IStrategy innerStrategy)
        {
            _innerStrategy = innerStrategy;
        }

        private IStrategy _innerStrategy;

        #region IStrategy Members

        public void DoSomething()
        {
            // TODO:  Add StrategyDecorator.DoSomething implementation
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj is StrategyDecorator)
            {
                StrategyDecorator peer = (StrategyDecorator) obj;
                return InnerStrategy.Equals(peer.InnerStrategy);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }


    public class CompoundStrategy : IStrategy
    {
        private IStrategy[] _innerStrategies;

        public IStrategy[] InnerStrategies
        {
            get { return _innerStrategies; }
        }

        public CompoundStrategy(IStrategy[] innerStrategies)
        {
            _innerStrategies = innerStrategies;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void DoSomething()
        {
            throw new NotImplementedException();
        }


        public override bool Equals(object obj)
        {
            if (obj is CompoundStrategy)
            {
                CompoundStrategy peer = (CompoundStrategy) obj;

                bool returnValue = (InnerStrategies.Length == peer.InnerStrategies.Length);

                if (returnValue)
                {
                    for (int i = 0; i < InnerStrategies.Length; i++)
                    {
                        returnValue = returnValue && (InnerStrategies[i].Equals(peer.InnerStrategies[i]));
                    }
                }

                return returnValue;
            }
            else
            {
                return false;
            }
        }
    }
}