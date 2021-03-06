﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using VendorManagement.Data.Models;

namespace VendorManagement.Data.Repos
{
    public class VendorRepo : IVendorRepository
    {
        private readonly string connString;

        public VendorRepo(IConfiguration configuration)
        {
            connString = configuration.GetValue<string>("Database:VendorManagement:ConnectionString");
        }

        internal IDbConnection DBConnection
        {
            get { return new NpgsqlConnection(connString); }
        }


        #region IVendorRepository

        public Vendor Add(Vendor item)
        {
            var result = new Vendor
            {
                Id = Guid.NewGuid(),
                Code = item.Code,
                Location = item.Location,
                Name = item.Name
            };

            using (IDbConnection conn = DBConnection)
            {
                conn.Open();
                conn.Execute("INSERT INTO vendor (id, code, name, location) VALUES (@Id, @Code, @Name, @Location);", result);
            }

            return result;
        }

        public IEnumerable<Vendor> FindAll()
        {
            using (IDbConnection conn = DBConnection)
            {
                conn.Open();
                return conn.Query<Vendor>("SELECT * FROM vendor WHERE deleted_at IS NULL;");
            }
        }

        public Vendor FindByCode(string code)
        {
            using (IDbConnection conn = DBConnection)
            {
                conn.Open();
                return conn.Query<Vendor>("SELECT * FROM vendor WHERE code=@code AND deleted_at IS NULL;", new { code }).FirstOrDefault();
            }
        }

        public Vendor FindById(Guid id)
        {
            using (IDbConnection conn = DBConnection)
            {
                conn.Open();
                return conn.Query<Vendor>("SELECT * FROM vendor WHERE id=@id AND deleted_at IS NULL;", new { id }).FirstOrDefault();
            }
        }

        public void Remove(Guid id)
        {
            using (IDbConnection conn = DBConnection)
            {
                conn.Open();
                conn.Execute("UPDATE vendor SET deleted_at=@timestamp WHERE id=@id;", new { id, timestamp = DateTime.UtcNow });
            }
        }

        public void Remove(string code)
        {
            using (IDbConnection conn = DBConnection)
            {
                conn.Open();
                conn.Execute("UPDATE vendor SET deleted_at=@timestamp WHERE code=@code;", new { code, timestamp = DateTime.UtcNow });
            };
        }

        public void Update(Vendor item)
        {
            using (IDbConnection conn = DBConnection)
            {
                conn.Open();
                conn.Execute("UPDATE vendor SET name=@Name, location=@Location WHERE code=@code AND deleted_at IS NULL;", item);
            }
        }

        #endregion
    }
}
