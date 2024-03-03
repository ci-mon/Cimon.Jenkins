﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cimon.Jenkins;

public abstract record StringQuery : Query<string>
{
	public override bool AddApiJsonSuffix => false;
	protected override async Task<string?> OnGetResult(HttpResponseMessage response, CancellationToken ctx) => 
		await response.Content.ReadAsStringAsync(ctx).ConfigureAwait(false);
}