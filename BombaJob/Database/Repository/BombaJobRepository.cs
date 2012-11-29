using System;
using System.Collections.ObjectModel;

using NHibernate;
using NHibernate.Criterion;

using BombaJob.Database.Domain;
using BombaJob.Utilities.Interfaces;

namespace BombaJob.Database
{
    public class BombaJobRepository : IBombaJobRepository
    {
        #region Text
        public void AddText(Text ent)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                Text t = this.GetTextById(ent.TextID);
                if (t != null)
                {
                    ent.ID = t.ID;
                    this.UpdateEntity(ent);
                }
                else
                    this.AddEntity(ent);
            }
        }

        public void UpdateText(Text ent)
        {
            this.UpdateEntity(ent);
        }

        public void RemoveText(Text ent)
        {
            this.DeleteEntity(ent);
        }

        public ObservableCollection<Text> GetTexts()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var txts = session
                            .CreateCriteria(typeof(Text))
                            .AddOrder(Order.Asc("TextID"))
                            .List<Text>();
                return new ObservableCollection<Text>(txts);
            }
        }

        public Text GetTextById(int textID)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                Text txt = session
                            .CreateCriteria(typeof(Text))
                            .Add(Restrictions.Eq("TextID", textID))
                            .UniqueResult<Text>();
                return txt;
            }
        }
        #endregion

        #region Category
        public void AddCategory(Category ent)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                Category t = this.GetCategoryById(ent.CategoryID);
                if (t != null)
                {
                    ent.ID = t.ID;
                    this.UpdateEntity(ent);
                }
                else
                    this.AddEntity(ent);
            }
        }

        public void UpdateCategory(Category ent)
        {
            this.UpdateEntity(ent);
        }

        public void RemoveCategory(Category ent)
        {
            this.DeleteEntity(ent);
        }

        public ObservableCollection<Category> GetCategories()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var cats = session
                            .CreateCriteria(typeof(Category))
                            .AddOrder(Order.Asc("Title"))
                            .List<Category>();
                return new ObservableCollection<Category>(cats);
            }
        }

        public ObservableCollection<Category> GetCategoriesFor(bool humanYn)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var cats = session.CreateCriteria<Category>()
                        .Add(Subqueries.PropertyIn("CategoryID",
                            DetachedCriteria.For<JobOffer>()
                                .Add(Restrictions.Eq("HumanYn", humanYn))
                                .SetProjection(Projections.Property("CategoryID"))
                            ))
                        .List<Category>();
                return new ObservableCollection<Category>(cats);
            }
        }

        public Category GetCategoryById(int entID)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                Category cat = session
                                .CreateCriteria(typeof(Category))
                                .Add(Restrictions.Eq("CategoryID", entID))
                                .UniqueResult<Category>();
                return cat;
            }
        }
        #endregion

        #region JobOffer
        public void AddJobOffer(JobOffer ent)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                JobOffer t = this.GetJobOfferById(ent.OfferID);
                if (t != null)
                {
                    ent.ID = t.ID;
                    this.UpdateEntity(ent);
                }
                else
                    this.AddEntity(ent);
            }
        }

        public void UpdateJobOffer(JobOffer ent)
        {
            this.UpdateEntity(ent);
        }

        public void RemoveJobOffer(JobOffer ent)
        {
            this.DeleteEntity(ent);
        }

        public ObservableCollection<JobOffer> GetNewestOffers()
        {
            return this.GetNewestOffers(AppSettings.OffersPerPageMax);
        }

        public ObservableCollection<JobOffer> GetNewestOffers(int offersLimit)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var offers = session
                            .CreateCriteria(typeof(JobOffer))
                            .AddOrder(Order.Desc("PublishDate"))
                            .SetMaxResults(offersLimit)
                            .List<JobOffer>();
                return new ObservableCollection<JobOffer>(offers);
            }
        }

        public ObservableCollection<JobOffer> GetJobOffers()
        {
            return this.GetJobOffers(AppSettings.OffersPerPageMax);
        }

        public ObservableCollection<JobOffer> GetJobOffers(int offersLimit)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var offers = session
                            .CreateCriteria(typeof(JobOffer))
                            .Add(Restrictions.Eq("HumanYn", false))
                            .AddOrder(Order.Desc("PublishDate"))
                            .SetMaxResults(offersLimit)
                            .List<JobOffer>();
                return new ObservableCollection<JobOffer>(offers);
            }
        }

        public ObservableCollection<JobOffer> GetPeopleOffers()
        {
            return this.GetPeopleOffers(AppSettings.OffersPerPageMax);
        }

        public ObservableCollection<JobOffer> GetPeopleOffers(int offersLimit)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var offers = session
                            .CreateCriteria(typeof(JobOffer))
                            .Add(Restrictions.Eq("HumanYn", true))
                            .AddOrder(Order.Desc("PublishDate"))
                            .SetMaxResults(offersLimit)
                            .List<JobOffer>();
                return new ObservableCollection<JobOffer>(offers);
            }
        }

        public JobOffer GetJobOfferById(int entID)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                JobOffer off = session
                               .CreateCriteria(typeof(JobOffer))
                               .Add(Restrictions.Eq("OfferID", entID))
                               .UniqueResult<JobOffer>();
                return off;
            }
        }

        public ObservableCollection<JobOffer> GetByCategoryID(int categoryID)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var offers = session
                            .CreateCriteria(typeof(JobOffer))
                            .Add(Restrictions.Eq("CategoryID", categoryID))
                            .AddOrder(Order.Desc("PublishDate"))
                            .List<JobOffer>();
                return new ObservableCollection<JobOffer>(offers);
            }
        }

        public ObservableCollection<JobOffer> SearchJobOffers(string searchKeyword)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var offers = session
                            .CreateCriteria(typeof(JobOffer))
                            .Add(Restrictions.Like("Title", searchKeyword, MatchMode.Anywhere) ||
                                 Restrictions.Like("Positivism", searchKeyword, MatchMode.Anywhere) ||
                                 Restrictions.Like("Negativism", searchKeyword, MatchMode.Anywhere))
                            .AddOrder(Order.Desc("PublishDate"))
                            .List<JobOffer>();
                return new ObservableCollection<JobOffer>(offers);
            }
        }

        public int GetJobOffersCount()
        {
            int offersCount = 0;
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var offers = session
                            .CreateCriteria(typeof(JobOffer))
                            .List<JobOffer>();
                offersCount = offers.Count;
            }
            return offersCount;
        }
        #endregion

        #region Private methods
        private void AddEntity(object ent)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(ent);
                    transaction.Commit();
                }
            }
        }

        private void UpdateEntity(object ent)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Update(ent);
                    transaction.Commit();
                }
            }
        }

        private void DeleteEntity(object ent)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Delete(ent);
                    transaction.Commit();
                }
            }
        }
        #endregion
    }
}
