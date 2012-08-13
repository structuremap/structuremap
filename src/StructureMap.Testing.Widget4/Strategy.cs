using System;

namespace StructureMap.Testing.Widget4
{
    public interface IStrategy
    {
        void DoSomething();
    }


    public class Strategy : IStrategy
    {
        public Strategy(string name, int count, double rating, long quantity, bool isCalculated)
        {
            Name = name;
            Count = count;
            Rating = rating;
            Quantity = quantity;
            IsCalculated = isCalculated;
        }

        public string Name { get; set; }

        public int Count { get; set; }

        public double Rating { get; set; }

        public long Quantity { get; set; }

        public bool IsCalculated { get; set; }

        #region IStrategy Members

        public void DoSomething()
        {
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj is Strategy)
            {
                var peer = (Strategy) obj;

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
        private readonly IStrategy _defaultStrategy;
        private readonly IStrategy[] _innerStrategies;
        private readonly string _name;
        private readonly long _quantity;
        private readonly StrategyType _strategyType;

        public ComplexStrategy(IStrategy[] innerStrategies, string name, long quantity,
                               IStrategy defaultStrategy, StrategyType strategyType)
        {
            _innerStrategies = innerStrategies;
            _name = name;
            _quantity = quantity;
            _defaultStrategy = defaultStrategy;
            _strategyType = strategyType;
        }

        #region IStrategy Members

        public void DoSomething()
        {
        }

        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is ComplexStrategy)
            {
                var peer = (ComplexStrategy) obj;
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
        public RandomStrategy(double seed)
        {
            Seed = seed;
        }

        public double Seed { get; set; }

        #region IStrategy Members

        public void DoSomething()
        {
        }

        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is RandomStrategy)
            {
                var peer = (RandomStrategy) obj;
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
        private readonly string _color;

        public ColorStrategy(string color)
        {
            _color = color;
        }

        public string Color { get { return _color; } }

        #region IStrategy Members

        public void DoSomething()
        {
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj is ColorStrategy)
            {
                var peer = (ColorStrategy) obj;
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
        public StrategyDecorator(IStrategy innerStrategy)
        {
            InnerStrategy = innerStrategy;
        }

        public IStrategy InnerStrategy { get; set; }

        #region IStrategy Members

        public void DoSomething()
        {
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj is StrategyDecorator)
            {
                var peer = (StrategyDecorator) obj;
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
        private readonly IStrategy[] _innerStrategies;

        public CompoundStrategy(IStrategy[] innerStrategies)
        {
            _innerStrategies = innerStrategies;
        }

        public IStrategy[] InnerStrategies { get { return _innerStrategies; } }

        #region IStrategy Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        public override bool Equals(object obj)
        {
            if (obj is CompoundStrategy)
            {
                var peer = (CompoundStrategy) obj;

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